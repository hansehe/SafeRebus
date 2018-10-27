using System;
using Microsoft.Extensions.Configuration;

namespace SafeRebus.Config
{
    public static class HostConfig
    {
        private const long DefaultCleaningOutboxTimerPeriodSec = 10;
        private const long DefaultCleanOldMessageIdsFromOutboxTimeThresholdSec = 30;
        private const int DefaultRequestsPerCycle = 10;
        
        public static TimeSpan GetHostCleaningOutboxCyclePeriod(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["host:cleaningOutboxCyclePeriodSec"], out var period))
            {
                period = DefaultCleaningOutboxTimerPeriodSec;
            }

            var timespanPeriod = TimeSpan.FromSeconds(period);
            return timespanPeriod;
        }
        
        public static TimeSpan GetHostCleanOldMessageIdsFromOutboxTimeThreshold(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["host:timeThresholdSecForCleaningOutbox"], out var threshold))
            {
                threshold = DefaultCleanOldMessageIdsFromOutboxTimeThresholdSec;
            }

            var timespanThreshold = TimeSpan.FromSeconds(threshold);
            return timespanThreshold;
        }
        
        public static int GetHostRequestsPerCycle(this IConfiguration configuration)
        {
            if (!int.TryParse(configuration["host:requestsPerCycle"], out var requests))
            {
                requests = DefaultRequestsPerCycle;
            }
            return requests;
        }
    }
}