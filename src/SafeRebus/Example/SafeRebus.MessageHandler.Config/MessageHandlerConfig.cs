using System;
using Microsoft.Extensions.Configuration;

namespace SafeRebus.MessageHandler.Config
{
    public static class MessageHandlerConfig
    {
        private const int DefaultRequestsPerCycle = 10;
        
        public static int GetHostRequestsPerCycle(this IConfiguration configuration)
        {
            if (!int.TryParse(configuration["messageHandlerHost:requestsPerCycle"], out var requests))
            {
                requests = DefaultRequestsPerCycle;
            }
            return requests;
        }
    }
}