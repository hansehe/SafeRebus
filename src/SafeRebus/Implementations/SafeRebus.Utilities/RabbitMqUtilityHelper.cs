using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public static class RabbitMqUtilityHelper
    {
        private const string DeclareWithRebusDirectExchangeType = "RebusDirect";
        private const string DirectExchangeType = "direct";
        private static readonly Dictionary<string, object> HighAvaiableQueueArguments = new Dictionary<string, object> { { "x-ha-policy", "all" } };
        
        public static string BuildRabbitMqConnectionString(this IConfiguration configuration)
        {
            var user = configuration.GetRabbitMqUser();
            var password = configuration.GetRabbitMqPassword();
            var hostname = configuration.GetRabbitMqHostname();
            var connectionString = $"amqp://{user}:{password}@{hostname}";
            return connectionString;
        }
        
        public static void WaitForAvailableRabbitMq(this IConnectionFactory connectionFactory, TimeSpan timeSpan, ILogger logger = null)
        {
            logger?.LogInformation($"Trying to connect to rabbitMq with url: {connectionFactory.Uri}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (stopwatch.Elapsed < timeSpan)
            {
                if (connectionFactory.TryConnectRabbitMq())
                {
                    logger?.LogInformation("Successfully connected with rabbitMq!");
                    return;
                }
            }

            const string errMsg =
                "Could not establish a connection with rabbitmq! Please review hostname, username and/or password.";
            logger?.LogError(errMsg);
            throw new Exception(errMsg);
        }
        
        public static uint PurgeQueue(this IConnectionFactory connectionFactory, string queue)
        {
            using (var conn = connectionFactory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                try
                {
                    var purgedMessages = model.QueuePurge(queue);
                    return purgedMessages;
                }
                catch
                {
                    // ignored
                }
            }
            return 0;
        }
        
        public static void DeleteQueue(this IConnectionFactory connectionFactory, string queue)
        {
            using (var conn = connectionFactory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                try
                {
                    model.QueueDeleteNoWait(queue);
                }
                catch
                {
                    // ignored
                }
            }
        }
        
        public static void CreateQueue(this IConnectionFactory connectionFactory, string queue)
        {
            using (var conn = connectionFactory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                try
                {
                    model.QueueDeclarePassive(queue);
                }
                catch (Exception e)
                {
                    // Queue does not exist, create it
                    GetOpenedModelIfClosed(conn, model)
                        .QueueDeclare(queue, true, false, false, HighAvaiableQueueArguments);
                }
                finally
                {
                    // Declare with routing bindings
                    try
                    {
                        model.ExchangeDeclarePassive(queue);
                        model.ExchangeDeclarePassive(DeclareWithRebusDirectExchangeType);
                    }
                    catch (Exception e)
                    {
                        var newModel = GetOpenedModelIfClosed(conn, model);
                        newModel.ExchangeDeclare(queue, DirectExchangeType, true);
                        newModel.ExchangeDeclare(DeclareWithRebusDirectExchangeType, DirectExchangeType, true);
                    }
                    finally
                    {
                        var newModel = GetOpenedModelIfClosed(conn, model);
                        newModel.QueueBind(queue, queue, queue, null);
                        newModel.QueueBind(queue, DeclareWithRebusDirectExchangeType, queue, null);
                    }
                }
            }
        }
        
        private static bool TryConnectRabbitMq(this IConnectionFactory connectionFactory)
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
        
        private static IModel GetOpenedModelIfClosed(IConnection conn, IModel model)
        {
            if (model.IsClosed)
            {
                model.Dispose();
                model = conn.CreateModel();
            }

            return model;
        }
    }
}