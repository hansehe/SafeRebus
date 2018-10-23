using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Rebus.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public class RabbitMqUtility : IRabbitMqUtility
    {
        private readonly IConfiguration Configuration;
        
        public RabbitMqUtility(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void WaitForAvailableRabbitMq()
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(ConnectionString);
            var timeout = TimeSpan.FromMilliseconds(Configuration.GetRabbitMqTimeoutMs());
            WaitForAvailableRabbitMq(timeout, connectionFactory);
        }

        public string ConnectionString => BuildConnectionString(Configuration);

        public string InputQueue => Configuration.GetRabbitMqInputQueue();

        public string OutputQueue => Configuration.GetRabbitMqOutputQueue();

        public LogLevel LogLevel => Configuration.GetRabbitMqLogLevel();


        private static string BuildConnectionString(IConfiguration configuration)
        {
            var user = configuration.GetRabbitMqUser();
            var password = configuration.GetRabbitMqPassword();
            var hostname = configuration.GetRabbitMqHostname();
            var connectionString = $"amqp://{user}:{password}@{hostname}";
            return connectionString;
        }
        
        private static void WaitForAvailableRabbitMq(TimeSpan timeSpan, IConnectionFactory connectionFactory)
        {
            Console.WriteLine($"Trying to connect to rabbitMq with url: {connectionFactory.Uri}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.Elapsed < timeSpan)
            {
                if (TryConnectRabbitMq(connectionFactory))
                {
                    Console.WriteLine("Successfully connected with rabbitMq!");
                    return;
                }
            }
            throw new Exception("Could not establish a connection with rabbitmq! Please review hostname, username and/or password.");
        }

        private static bool TryConnectRabbitMq(IConnectionFactory connectionFactory)
        {
            try
            {
                var connection = connectionFactory.CreateConnection();
                connection.Close();
                connection.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}