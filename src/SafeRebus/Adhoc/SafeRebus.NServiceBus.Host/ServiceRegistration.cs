using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.Extensions.Builder;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.NServiceBus.Host.MessageHandlers;
using SafeRebus.Utilities;

namespace SafeRebus.NServiceBus.Host
{
    public static class ServiceRegistration
    {
        public static IServiceCollection ConfigureWithNServiceBusHost(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .AddScoped<IHandleMessages<DummyRequest>, DummyRequestMessageHandler>()
                .AddScoped<IHandleMessages<SafeRebusRequest>, SafeRebusRequestMessageHandler>()
                .AddScoped<IRabbitMqUtility, RabbitMqUtility>()
                .UseNServiceBus()
                .AddHostedService<NServiceBusHost>()
                .UseNServiceBusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }

        private static IServiceCollection UseNServiceBus(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IEndpointInstance>(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var rabbitMqUtility = provider.GetService<IRabbitMqUtility>();
                rabbitMqUtility.CreateQueue(configuration.GetRabbitMqInputQueue());
                
                var endpointConfiguration = new EndpointConfiguration(configuration.GetRabbitMqInputQueue());
                
                endpointConfiguration.Conventions()
                    .DefiningMessagesAs(
                        type => type.Namespace == typeof(DummyRequest).Namespace)
                    .DefiningMessagesAs(
                        type => type.Namespace == typeof(SafeRebusRequest).Namespace)
                    .DefiningMessagesAs(
                        type => type.Namespace == typeof(SafeRebusResponse).Namespace);
                
                endpointConfiguration.UseTransport<RabbitMQTransport>()
                    .ConnectionString(configuration.GetNServiceBusConnectionString())
                    .UseDirectRoutingTopology()
                    .Routing();

                var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
                return endpointInstance;
            });
        }
        
        private static IServiceCollection UseNServiceBusConfiguration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection.AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .AddDefaultSafeRebusConfiguration()
                .AddJsonFile(Config.DefaultConfig)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build());
        }
    }
}