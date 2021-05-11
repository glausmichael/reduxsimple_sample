namespace ReduxSimple.Redux
{
    public abstract class EffectBase
    {
        public abstract Effect<RootState> CreateEffect();
    }
}
