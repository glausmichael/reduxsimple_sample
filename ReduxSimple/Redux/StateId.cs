using System;

namespace ReduxSimple.Redux
{
    public static class StateId
    {
        public static string GetId<TState>()
        {
            return GetIdByType(typeof(TState));
        }

        public static string GetId(IMainFeatureState mainFeatureState)
        {
            return GetIdByType(mainFeatureState.GetType());
        }

        private static string GetIdByType(Type featureStateType)
        {
            return featureStateType.Name;
        }
    }
}
