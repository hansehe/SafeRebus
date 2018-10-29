using System;
using Microsoft.Extensions.Configuration;

namespace SafeRebus.Outbox.Config
{
    public static class OutboxConfig
    {
        private const long DefaultCleaningOutboxTimerPeriodSec = 10;
        private const long DefaultCleanOldMessageIdsFromOutboxTimeThresholdSec = 30;
        
        public static TimeSpan GetOutboxCleaningOutboxCyclePeriod(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["outboxHost:cleaningOutboxCyclePeriodSec"], out var period))
            {
                period = DefaultCleaningOutboxTimerPeriodSec;
            }

            var timespanPeriod = TimeSpan.FromSeconds(period);
            return timespanPeriod;
        }
        
        public static TimeSpan GetOutboxCleanOldMessageIdsFromOutboxTimeThreshold(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["outboxHost:timeThresholdSecForCleaningOutbox"], out var threshold))
            {
                threshold = DefaultCleanOldMessageIdsFromOutboxTimeThresholdSec;
            }

            var timespanThreshold = TimeSpan.FromSeconds(threshold);
            return timespanThreshold;
        }
    }
}