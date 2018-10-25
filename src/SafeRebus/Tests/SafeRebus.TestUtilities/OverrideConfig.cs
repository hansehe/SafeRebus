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
                {"database:pooling", true.ToString()},
                {"host:sendDummyRequests", false.ToString()},
                {"host:pauseBetweenRequestsMs", 500.ToString()},
                {"host:requestsPerCycle", 10.ToString()},
            };

            BaseConfig.DropJokerExceptions = false;
            
            return overrideDict;
        }
    }
}