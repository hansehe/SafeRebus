using System;
using System.Collections.Generic;
using SafeRebus.TestUtilities;

namespace SafeRebus.Adapter.NServiceBus.Tests
{
    public static class NServiceBusOverrideConfig
    {
        public static Dictionary<string, string> GetNServiceBusAdditionalOverrideConfig()
        {
            var random = new Random();
            var number = random.Next();
            var randomInputQueue = $"NServiceBus.InputQueue_{number.ToString()}";
            var randomOutputQueue = $"NServiceBus.OutputQueue_{number.ToString()}";
            var overrideDict = new Dictionary<string, string>
            {
                ["rabbitMq:inputQueue"] = randomInputQueue, 
                ["rabbitMq:outputQueue"] = randomOutputQueue
            };

            return overrideDict;
        }
    }
}