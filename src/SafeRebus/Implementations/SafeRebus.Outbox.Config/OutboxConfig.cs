using System;
using Microsoft.Extensions.Configuration;

namespace SafeRebus.Outbox.Config
{
    public static class OutboxConfig
    {
        public const string DefaultOutboxConfigFilename = "DefaultOutboxConfig.json";
        
        private const long DefaultCleaningOutboxTimerPeriodSec = 2;
        private const long DefaultCleanOldMessageIdsFromDuplicationFilterTimeThresholdSec = 30;
        private const long DefaultCleanOutboxMessagesTimeThresholdSec = 5;
        
        public static TimeSpan GetCleaningOutboxCyclePeriod(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["outboxHost:cleaningOutboxCyclePeriodSec"], out var period))
            {
                period = DefaultCleaningOutboxTimerPeriodSec;
            }

            var timespanPeriod = TimeSpan.FromSeconds(period);
            return timespanPeriod;
        }
        
        public static TimeSpan GetCleanOldMessageIdsFromDuplicationFilterTimeThreshold(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["outboxHost:timeThresholdSecForCleaningOutboxDuplicationFilter"], out var threshold))
            {
                threshold = DefaultCleanOldMessageIdsFromDuplicationFilterTimeThresholdSec;
            }

            var timespanThreshold = TimeSpan.FromSeconds(threshold);
            return timespanThreshold;
        }
        
        public static TimeSpan GetCleanOutboxMessagesTimeThreshold(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["outboxHost:timeThresholdSecForCleaningOutboxMessages"], out var threshold))
            {
                threshold = DefaultCleanOutboxMessagesTimeThresholdSec;
            }

            var timespanThreshold = TimeSpan.FromSeconds(threshold);
            return timespanThreshold;
        }
    }
}