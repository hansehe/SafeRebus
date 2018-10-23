using System;
using Microsoft.Extensions.Configuration;
using Rebus.Logging;

namespace SafeRebus.Config
{
    public static class Config
    {
        private const string DefaultConfigFilename = "DefaultConfig.json";
        private const string DefaultConfigDockerFilename = "DefaultConfig.Docker.json";
        
        private static bool InContainer => 
            Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER") == "true";

        public static string GetConfigFilename => InContainer ? DefaultConfigDockerFilename : DefaultConfigFilename;
        
        public static long GetRabbitMqTimeoutMs(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["rabbitMq:timeoutMs"], out var timeoutMs))
            {
                timeoutMs = DefaultConfig.DefaultRabbitMqTimeoutMs;
            }
            return timeoutMs;
        }

        public static string GetRabbitMqUser(this IConfiguration configuration)
        {
            return configuration["rabbitMq:user"] ?? DefaultConfig.DefaultRabbitMqUser;
        }
        
        public static string GetRabbitMqPassword(this IConfiguration configuration)
        {
            return configuration["rabbitMq:password"] ?? DefaultConfig.DefaultRabbitMqPassword;
        }
        
        public static string GetRabbitMqHostname(this IConfiguration configuration)
        {
            return configuration["rabbitMq:hostname"] ?? DefaultConfig.DefaultRabbitMqHostname;
        }
        
        public static LogLevel GetRabbitMqLogLevel(this IConfiguration configuration)
        {
            if (!Enum.TryParse<LogLevel>(configuration["rabbitMq:logLevel"], out var logLevel))
            {
                logLevel = DefaultConfig.DefaultRabbitMqLogLevel;
            }
            return logLevel;
        }

        public static string GetRabbitMqInputQueue(this IConfiguration configuration)
        {
            return configuration["rabbitMq:inputQueue"] ?? DefaultConfig.DefaultRabbitMqInputQueue;
        }
        
        public static string GetRabbitMqOutputQueue(this IConfiguration configuration)
        {
            return configuration["rabbitMq:outputQueue"] ?? DefaultConfig.DefaultRabbitMqOutputQueue;
        }
    }
}