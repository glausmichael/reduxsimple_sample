using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ReduxSimple.Redux
{
    public static class AttachedStates
    {
        private static readonly ConditionalWeakTable<object, Dictionary<Type, object>> AttachedStatesTable = new ConditionalWeakTable<object, Dictionary<Type, object>>();

        public static T GetAttachedState<T>(object targetState) where T : new()
        {
            if (!AttachedStatesTable.TryGetValue(targetState, out var attachedStates))
            {
                attachedStates = new Dictionary<Type, object>();
                AttachedStatesTable.Add(targetState, attachedStates);

                return AddAttachedState<T>(attachedStates);
            }

            if (!attachedStates.TryGetValue(typeof(T), out var attachedState))
            {
                return AddAttachedState<T>(attachedStates);
            }

            return (T)attachedState;
        }

        private static T AddAttachedState<T>(Dictionary<Type, object> stateExtensions) where T : new()
        {
            var attachedState = Activator.CreateInstance<T>();
            stateExtensions.Add(typeof(T), attachedState);
            return attachedState;
        }
    }
}
