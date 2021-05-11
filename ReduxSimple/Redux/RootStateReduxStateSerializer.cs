using Newtonsoft.Json;
using ReduxSimple.Redux.DevTools;
using SuccincT.JSON;
using System.Text;

namespace ReduxSimple.Redux
{
    class RootStateReduxStateSerializer : IReduxStateSerializer
    {
        public string Serialize(object state)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = SuccinctContractResolver.Instance,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var rootState = (RootState)state;

            var serializedSateSb = new StringBuilder();
            foreach (var featureState in rootState.GetAllFeatureStates())
            {
                serializedSateSb.AppendLine($"Feature: {featureState.GetType().Name}");
                serializedSateSb.Append(JsonConvert.SerializeObject(featureState, serializerSettings));
            }

            return serializedSateSb.ToString();
        }
    }
}
