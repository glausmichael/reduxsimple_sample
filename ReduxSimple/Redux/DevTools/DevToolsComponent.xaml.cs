using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using static ReduxSimple.Selectors;
using static ReduxSimple.DevTools.Selectors;
using static ReduxSimple.DevTools.Reducers;
using Newtonsoft.Json;
using SuccincT.JSON;
using ReduxSimple.DevTools;
using System.Collections.Immutable;

namespace ReduxSimple.Redux.DevTools
{
    /// <summary>
    /// Interaction logic for DevToolsComponent.xaml
    /// </summary>
    public partial class DevToolsComponent : UserControl
    {
        private readonly ReduxStore<DevToolsState> devToolsStore = new ReduxStore<DevToolsState>(CreateReducers());

        public DevToolsComponent()
        {
            InitializeComponent();
        }

        private void ReduxActionInfosListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null)
            {
                this.devToolsStore.Dispatch(new SelectPositionAction { Position = this.ReduxActionInfosListView.Items.IndexOf(item.Content) });
            }
        }

        internal void Initialize<TState>(ReduxStore<TState> store, DevToolsConfiguration configuration) where TState : class, new()
        {
            // Observe UI events
            UndoButton.Click += (sender, e) => store.Undo();
            RedoButton.Click += (sender, e) => store.Redo();
            ResetButton.Click += (sender, e) => store.Reset();

            PlayPauseButton.Click += (sender, e) => this.devToolsStore.Dispatch(new TogglePlayPauseAction());

            Slider.ValueChanged += (sender, e) =>
                {
                    if (!this.Slider.IsFocused)
                    {
                        return;
                    }

                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this.Slider), null);
                    Keyboard.ClearFocus();

                    this.devToolsStore.Dispatch(new MoveToPositionAction { Position = (int)e.NewValue });
                };

            // Observe changes on DevTools state
            Observable.CombineLatest(
                this.devToolsStore.Select(SelectCurrentPosition),
                this.devToolsStore.Select(SelectPlaySessionActive),
                this.devToolsStore.Select(SelectMaxPosition),
                store.ObserveCanUndo(),
                store.ObserveCanRedo(),
                Tuple.Create
            )
                .ObserveOnDispatcher()
                .Subscribe(x =>
                {
                    var (currentPosition, playSessionActive, maxPosition, canUndo, canRedo) = x;

                    Slider.Value = currentPosition;
                    Slider.Maximum = maxPosition;

                    if (playSessionActive)
                    {
                        UndoButton.IsEnabled = false;
                        RedoButton.IsEnabled = false;
                        ResetButton.IsEnabled = false;
                        PlayPauseButton.IsEnabled = true;
                        Slider.IsEnabled = false;
                        PlayPauseButton.Content = "\xE769";
                    }
                    else
                    {
                        UndoButton.IsEnabled = canUndo;
                        RedoButton.IsEnabled = canRedo;
                        ResetButton.IsEnabled = canUndo || canRedo;
                        PlayPauseButton.IsEnabled = canRedo;
                        Slider.IsEnabled = maxPosition > 0;
                        PlayPauseButton.Content = "\xE768";
                    }
                });

            this.devToolsStore.Select(
                CombineSelectors(SelectCurrentActions, SelectSelectedActionPosition)
            )
                .ObserveOnDispatcher()
                .Subscribe(x =>
                {
                    var (actions, selectedPosition) = x;

                    ReduxActionInfosListView.ItemsSource = actions;

                    var selectedIndex = Math.Min(selectedPosition, actions.Count - 1);
                    selectedIndex = Math.Max(selectedIndex, -1);

                    ReduxActionInfosListView.SelectedIndex = selectedIndex;
                });

            this.devToolsStore.Select(SelectSelectedReduxAction)
                .ObserveOnDispatcher()
                .Subscribe(reduxActionOption =>
                {
                    reduxActionOption.Match()
                        .Some().Do(reduxAction =>
                        {
                            var serializerSettings = new JsonSerializerSettings
                            {
                                ContractResolver = SuccinctContractResolver.Instance,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                Formatting = Formatting.Indented
                            };

                            SelectedReduxActionDataTextBlock.Text = JsonConvert.SerializeObject(
                                reduxAction.Data,
                                serializerSettings
                            );

                            var newStateString = configuration.StateSerializer.Serialize(reduxAction.NextState);
                            var oldStateString = configuration.StateSerializer.Serialize(reduxAction.PreviousState);

                            SelectedStateTextBlock.Text = newStateString;
                            this.DiffView.NewText = newStateString;
                            this.DiffView.OldText = oldStateString;
                        })
                        .None().Do(() =>
                        {
                            SelectedReduxActionDataTextBlock.Text = string.Empty;
                            SelectedStateTextBlock.Text = string.Empty;
                            this.DiffView.NewText = string.Empty;
                            this.DiffView.OldText = string.Empty;
                        })
                        .Exec();
                });

            this.devToolsStore.ObserveAction<MoveToPositionAction>()
                .WithLatestFrom(
                    this.devToolsStore.Select(SelectCurrentPosition),
                    Tuple.Create
                )
                .Subscribe(x =>
                {
                    var (action, currentPosition) = x;

                    if (action.Position < currentPosition)
                    {
                        for (int i = 0; i < currentPosition - action.Position; i++)
                        {
                            store.Undo();
                        }
                    }
                    if (action.Position > currentPosition)
                    {
                        for (int i = 0; i < action.Position - currentPosition; i++)
                        {
                            store.Redo();
                        }
                    }
                });

            this.devToolsStore.Select(SelectPlaySessionActive)
                .Select(playSessionActive =>
                    playSessionActive ? Observable.Interval(TimeSpan.FromSeconds(1)) : Observable.Empty<long>()
                )
                .Switch()
                .ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    bool canRedo = store.Redo();
                    if (!canRedo)
                    {
                        this.devToolsStore.Dispatch(new TogglePlayPauseAction());
                    }
                });

            // Observe changes on listened state
            var storeHistoryAtInitialization = store.GetHistory();

            store.ObserveHistory()
                .StartWith(storeHistoryAtInitialization)
                .Subscribe(historyInfos =>
                {
                    var mementosOrderedByDate = historyInfos.PreviousStates
                        .OrderBy(reduxMemento => reduxMemento.Date)
                        .ToList();

                    // Set list of current actions
                    // Set list of future (undone) actions
                    this.devToolsStore.Dispatch(new HistoryUpdated
                    {
                        CurrentActions = mementosOrderedByDate
                            .Select((reduxMemento, index) =>
                            {
                                int nextIndex = index + 1;
                                var nextState = nextIndex < mementosOrderedByDate.Count
                                    ? mementosOrderedByDate[nextIndex].PreviousState
                                    : store.State;

                                return new ReduxActionInfo
                                {
                                    Date = reduxMemento.Date,
                                    Type = reduxMemento.Action.GetType(),
                                    Data = reduxMemento.Action,
                                    PreviousState = reduxMemento.PreviousState,
                                    NextState = nextState
                                };
                            })
                            .ToImmutableList(),
                        FutureActions = historyInfos.FutureActions
                            .Select(action =>
                            {
                                return new ReduxActionInfo
                                {
                                    Type = action.GetType(),
                                    Data = action
                                };
                            })
                            .ToImmutableList()
                    });
                });

            this.devToolsStore.Dispatch(
                new SelectPositionAction { Position = storeHistoryAtInitialization.PreviousStates.Count - 1 }
            );
        }
    }
}
