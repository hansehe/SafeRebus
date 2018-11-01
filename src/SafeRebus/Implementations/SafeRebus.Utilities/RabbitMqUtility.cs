using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SafeRebus.Abstractions;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public class RabbitMqUtility : IRabbitMqUtility
    {
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        
        public RabbitMqUtility(
            ILogger<RabbitMqUtility> logger,
            IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        public void WaitForAvailableRabbitMq()
        {
            var timeout = TimeSpan.FromMilliseconds(Configuration.GetRabbitMqTimeoutMs());
            GetConnectionFactory().WaitForAvailableRabbitMq(timeout, Logger);
        }
        
        public void PurgeInputQueue()
        {
            var purgedMessages = GetConnectionFactory().PurgeQueue(InputQueue);
            Logger.LogInformation($"Purged input queue {InputQueue} with {purgedMessages} hanging messages.");
        }

        public void DeleteInputQueue()
        {
            GetConnectionFactory().DeleteQueue(InputQueue);
            Logger.LogInformation($"Deleted input queue {InputQueue}.");
        }

        public void CreateQueue(string queue)
        {
            GetConnectionFactory().CreateQueue(queue);
        }

        public string ConnectionString => Configuration.BuildRabbitMqConnectionString();

        public string InputQueue => Configuration.GetRabbitMqInputQueue();

        public string OutputQueue => Configuration.GetRabbitMqOutputQueue();

        public Rebus.Logging.LogLevel LogLevel => Configuration.GetRabbitMqLogLevel();

        private IConnectionFactory GetConnectionFactory() => new ConnectionFactory
        {
            Uri = new Uri(ConnectionString)
        };
    }
}