namespace ReduxSimple.Redux
{
    public interface IMainFeatureStateFactory<TFeature> where TFeature : IMainFeatureState
    {
    }

    public interface IMainFeatureStateFactory
    {
        IMainFeatureState CreateState();
    }
}
