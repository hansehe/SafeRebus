using System;
using System.Collections.Generic;
using SafeRebus.MessageHandler.Config;

namespace SafeRebus.TestUtilities
{
    public static class OverrideConfig
    {
        public static TimeSpan DurationOfAcidTest = TimeSpan.FromSeconds(10);
        
        public static Dictionary<string, string> GetOverrideConfig()
        {
            var random = new Random();
            var randomQueue = $"SafeRebus.InputQueue_{random.Next().ToString()}";
            var overrideDict = new Dictionary<string, string>
            {
                {"rabbitMq:inputQueue", randomQueue},
                {"rabbitMq:outputQueue", randomQueue},
                {"host:requestsPerCycle", 10.ToString()},
                {"outboxHost:timeThresholdSecForCleaningOutboxMessages", 1.ToString()},
                {"database:schema", DatabaseFixture.MigratedDatabaseSchema}
            };

            JokerExceptionsConfig.UseJokerExceptions = true;
            JokerExceptionsConfig.JokerExceptionProbabilityInPercent = 1;
            
            return overrideDict;
        }
    }
}