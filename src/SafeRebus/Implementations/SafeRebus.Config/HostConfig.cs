using System;
using Microsoft.Extensions.Configuration;

namespace SafeRebus.Config
{
    public static class HostConfig
    {
        private const long DefaultCleaningOutboxTimerPeriodSec = 10;
        private const long DefaultCleanOldMessageIdsFromOutboxTimeThresholdSec = 30;
        private const int DefaultRequestsPerCycle = 10;
        private const bool DefaultShouldSendDummyRequests = false;
        private const bool DefaultShouldCleanOutbox = true;
        
        private static bool OverrideSendDummyRequests => 
            Environment.GetEnvironmentVariable("SPAM_WITH_DUMMY_REQUESTS") == "true";
        
        public static TimeSpan GetHostCleaningOutboxTimerPeriod(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["host:cleaningOutboxTimerPeriodSec"], out var period))
            {
                period = DefaultCleaningOutboxTimerPeriodSec;
            }

            var timespanPeriod = TimeSpan.FromSeconds(period);
            return timespanPeriod;
        }
        
        public static TimeSpan GetHostCleanOldMessageIdsFromOutboxTimeThresholdSec(this IConfiguration configuration)
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
        
        public static bool HostShouldSendDummyRequests(this IConfiguration configuration)
        {
            if (!bool.TryParse(configuration["host:sendDummyRequests"], out var shouldSendDummyRequests))
            {
                shouldSendDummyRequests = DefaultShouldSendDummyRequests;
            }
            return shouldSendDummyRequests || OverrideSendDummyRequests;
        }
        
        public static bool HostShouldCleanOutbox(this IConfiguration configuration)
        {
            if (!bool.TryParse(configuration["host:shouldCleanOutbox"], out var cleanOutbox))
            {
                cleanOutbox = DefaultShouldCleanOutbox;
            }
            return cleanOutbox;
        }
    }
}