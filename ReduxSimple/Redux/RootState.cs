using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Unity;

namespace ReduxSimple.Redux
{
    public class RootState
    {
        private readonly ImmutableDictionary<string, IMainFeatureState> subStates;

        public RootState()
        {
        }

        public RootState(IEnumerable<IMainFeatureState> subStates)
        {
            var subStatesDict = new Dictionary<string, IMainFeatureState>();
            foreach (var subState in subStates)
            {
                subStatesDict[StateId.GetId(subState)] = subState;
            }

            this.subStates = subStatesDict.ToImmutableDictionary();
        }

        private RootState(ImmutableDictionary<string, IMainFeatureState> subStates)
        {
            this.subStates = subStates;
        }

        public TMainFeatureState GetStateByType<TMainFeatureState>() where TMainFeatureState : IMainFeatureState
        {
            return (TMainFeatureState)this.subStates[StateId.GetId<TMainFeatureState>()];
        }

        public RootState CopyWith<TMainFeatureState>(TMainFeatureState featureState) where TMainFeatureState : IMainFeatureState
        {
            return new RootState(this.subStates.SetItem(StateId.GetId<TMainFeatureState>(), featureState));
        }

        public IEnumerable<IMainFeatureState> GetAllFeatureStates()
        {
            return this.subStates.Values;
        }

        public static RootState GetInitialState(IUnityContainer container)
        {
            var featureAssembly = typeof(RootState).Assembly;

            var mainFeatureStateTypes = featureAssembly.GetTypes()
                .Where(t => typeof(IMainFeatureState).IsAssignableFrom(t) && !t.IsAbstract);

            var mainFeatureStates = new List<IMainFeatureState>();
            foreach (var mainFeatureStateType in mainFeatureStateTypes)
            {
                var featureFactoryInterfaceType = typeof(IMainFeatureStateFactory<>).MakeGenericType(mainFeatureStateType);
                var featureFactoryType = featureAssembly.GetTypes().FirstOrDefault(t => featureFactoryInterfaceType.IsAssignableFrom(t) && !t.IsAbstract);

                IMainFeatureState mainFeatureState;
                if (featureFactoryType == null)
                {
                    mainFeatureState = (IMainFeatureState)Activator.CreateInstance(mainFeatureStateType);
                }
                else
                {
                    mainFeatureState = ((IMainFeatureStateFactory)container.Resolve(featureFactoryType)).CreateState();
                }

                mainFeatureStates.Add(mainFeatureState);
            }

            return new RootState(mainFeatureStates);
        }
    }
}
