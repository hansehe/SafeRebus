using System;
using Microsoft.Extensions.Configuration;
using Rebus.Logging;

namespace SafeRebus.Config
{
    public static class RebusConfig
    {
        private static long DefaultRabbitMqTimeoutMs = 5000;
        private static string DefaultRabbitMqUser = "guest";
        private static string DefaultRabbitMqPassword = "guest";
        private static string DefaultRabbitMqHostname = "localhost";
        private static LogLevel DefaultRabbitMqLogLevel = LogLevel.Info;
        private static string DefaultRabbitMqInputQueue = "SafeRebus.InputQueue";
        private static string DefaultRabbitMqOutputQueue = "SafeRebus.OutputQueue";
        
        public static long GetRabbitMqTimeoutMs(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["rabbitMq:timeoutMs"], out var timeoutMs))
            {
                timeoutMs = DefaultRabbitMqTimeoutMs;
            }
            return timeoutMs;
        }

        public static string GetRabbitMqUser(this IConfiguration configuration)
        {
            return configuration["rabbitMq:user"] ?? DefaultRabbitMqUser;
        }
        
        public static string GetRabbitMqPassword(this IConfiguration configuration)
        {
            return configuration["rabbitMq:password"] ?? DefaultRabbitMqPassword;
        }
        
        public static string GetRabbitMqHostname(this IConfiguration configuration)
        {
            return configuration["rabbitMq:hostname"] ?? DefaultRabbitMqHostname;
        }
        
        public static LogLevel GetRabbitMqLogLevel(this IConfiguration configuration)
        {
            if (!Enum.TryParse<LogLevel>(configuration["rabbitMq:logLevel"], out var logLevel))
            {
                logLevel = DefaultRabbitMqLogLevel;
            }
            return logLevel;
        }

        public static string GetRabbitMqInputQueue(this IConfiguration configuration)
        {
            return configuration["rabbitMq:inputQueue"] ?? DefaultRabbitMqInputQueue;
        }
        
        public static string GetRabbitMqOutputQueue(this IConfiguration configuration)
        {
            return configuration["rabbitMq:outputQueue"] ?? DefaultRabbitMqOutputQueue;
        }
    }
}