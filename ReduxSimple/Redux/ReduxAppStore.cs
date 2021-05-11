using ReduxSimple.Redux.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace ReduxSimple.Redux
{
    public class ReduxAppStore : IStore
    {
        private ReduxStore<RootState> store;

        public ReduxAppStore()
        {
                
        }

        public void InitializeStore(IUnityContainer container)
        {
            this.store = new ReduxStore<RootState>(CreateReducers(), RootState.GetInitialState(container), true);

            var childContainer = container.CreateChildContainer();
            childContainer.RegisterInstance(this.store);

            this.RegisterEffects(childContainer);
        }

        public void Dispatch(object action)
        {
            this.store.Dispatch(action);
        }

        public IObservable<RootState> Select()
        {
            return this.store.Select();
        }

        public IObservable<TResult> Select<TResult>(Func<RootState, TResult> selector)
        {
            return this.store.Select(selector);
        }

        public IObservable<TResult> Select<TResult>(ISelectorWithoutProps<RootState, TResult> selector)
        {
            return this.store.Select(selector);
        }

        public IObservable<TResult> Select<TProps, TResult>(ISelectorWithProps<RootState, TProps, TResult> selector, TProps props)
        {
            return this.store.Select(selector, props);
        }

        public void OpenDevTools()
        {
            var devToolsConfiguration = new DevToolsConfiguration
            {
                StateSerializer = new RootStateReduxStateSerializer()
            };

            this.store.OpenDevTools(devToolsConfiguration);
        }

        private static IEnumerable<On<RootState>> CreateReducers()
        {
            var featureAssembly = typeof(RootState).Assembly;

            var reducerProvderTypes = featureAssembly.GetTypes()
                .Where(t => typeof(IReducerProvider).IsAssignableFrom(t) && !t.IsAbstract);

            var allReducers = new List<On<RootState>>();
            foreach (var reducerProviderType in reducerProvderTypes)
            {
                var reducerProvder = (IReducerProvider)Activator.CreateInstance(reducerProviderType);
                allReducers.AddRange(reducerProvder.GetReducers());
            }

            return allReducers;
        }

        private void RegisterEffects(IUnityContainer unityContainer)
        {
            var featureAssembly = typeof(RootState).Assembly;

            var effectsTypes = featureAssembly.GetTypes()
                .Where(t => typeof(EffectBase).IsAssignableFrom(t) && !t.IsAbstract);

            var effectsBases = effectsTypes.Select(effectsType => (EffectBase)unityContainer.Resolve(effectsType));

            this.store.RegisterEffects(effectsBases.Select(effectsHolder => effectsHolder.CreateEffect()).ToArray());
        }
    }
}
