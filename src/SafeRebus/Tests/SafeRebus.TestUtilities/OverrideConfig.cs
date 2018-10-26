using System;
using System.Collections.Generic;
using SafeRebus.Config;

namespace SafeRebus.TestUtilities
{
    public static class OverrideConfig
    {
        public static Dictionary<string, string> GetOverrideConfig()
        {
            var random = new Random();
            var randomQueue = $"SafeRebus.InputQueue_{random.Next().ToString()}";
            var randomSchema = $"SafeRebus_{random.Next().ToString()}";
            var overrideDict = new Dictionary<string, string>
            {
                {"rabbitMq:inputQueue", randomQueue},
                {"rabbitMq:outputQueue", randomQueue},
                {"database:schema", randomSchema},
                {"host:sendDummyRequests", true.ToString()},
                {"host:shouldCleanOutbox", true.ToString()},
                {"host:requestsPerCycle", 100.ToString()},
            };

            BaseConfig.UseJokerExceptions = true;
            
            return overrideDict;
        }
    }
}