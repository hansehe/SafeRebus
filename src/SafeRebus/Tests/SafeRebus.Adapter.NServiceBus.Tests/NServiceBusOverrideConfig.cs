using System;
using System.Collections.Generic;
using SafeRebus.TestUtilities;

namespace SafeRebus.Adapter.NServiceBus.Tests
{
    public static class NServiceBusOverrideConfig
    {
        public static Dictionary<string, string> GetNServiceBusOverrideConfig()
        {
            var random = new Random();
            var number = random.Next();
            var randomInputQueue = $"NServiceBus.InputQueue_{number.ToString()}";
            var randomOutputQueue = $"NServiceBus.OutputQueue_{number.ToString()}";
            var overrideDict = OverrideConfig.GetOverrideConfig();
            overrideDict["rabbitMq:inputQueue"] = randomInputQueue;
            overrideDict["rabbitMq:outputQueue"] = randomOutputQueue;
            
            return overrideDict;
        }
    }
}