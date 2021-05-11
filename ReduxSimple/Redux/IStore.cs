using System;

namespace ReduxSimple.Redux
{
    public interface IStore
    {
        void Dispatch(object action);

        IObservable<RootState> Select();

        IObservable<TResult> Select<TResult>(Func<RootState, TResult> selector);

        IObservable<TResult> Select<TResult>(ISelectorWithoutProps<RootState, TResult> selector);

        IObservable<TResult> Select<TProps, TResult>(ISelectorWithProps<RootState, TProps, TResult> selector, TProps props);

        void OpenDevTools();
    }
}
