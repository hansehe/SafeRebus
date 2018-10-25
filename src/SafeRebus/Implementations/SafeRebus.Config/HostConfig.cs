using System;
using Microsoft.Extensions.Configuration;

namespace SafeRebus.Config
{
    public static class HostConfig
    {
        private const long DefaultPauseBetweenRequestsMs = 1000;
        private const int DefaultRequestsPerCycle = 10;
        private const bool DefaultShouldSendDummyRequests = false;
        
        private static bool OverrideSendDummyRequests => 
            Environment.GetEnvironmentVariable("SPAM_WITH_DUMMY_REQUESTS") == "true";
        
        public static long GetHostPauseBetweenRequestsMs(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["host:pauseBetweenRequestsMs"], out var ms))
            {
                ms = DefaultPauseBetweenRequestsMs;
            }
            return ms;
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
    }
}