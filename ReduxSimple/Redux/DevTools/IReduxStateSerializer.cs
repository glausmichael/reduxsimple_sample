namespace ReduxSimple.Redux.DevTools
{
    public interface IReduxStateSerializer
    {
        string Serialize(object state);
    }
}