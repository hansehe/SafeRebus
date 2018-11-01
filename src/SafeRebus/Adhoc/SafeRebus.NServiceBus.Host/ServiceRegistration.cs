using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.Extensions.Builder;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.NServiceBus.Host.Contracts;
using SafeRebus.NServiceBus.Host.MessageHandlers;
using SafeRebus.Utilities;

namespace SafeRebus.NServiceBus.Host
{
    public static class ServiceRegistration
    {
        public static IServiceCollection ConfigureWithNServiceBusHost(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .AddScoped<IHandleMessages<NServiceBusDummyRequest>, DummyRequestMessageHandler>()
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
                var routing = endpointConfiguration.UseTransport<RabbitMQTransport>()
                    .ConnectionString(configuration.GetNServiceBusConnectionString())
                    .UseDirectRoutingTopology()
                    .Routing();
                routing.RouteToEndpoint(typeof(NServiceBusDummyRequest), configuration.GetRabbitMqOutputQueue());
                routing.RouteToEndpoint(typeof(NServiceBusResponse), configuration.GetRabbitMqOutputQueue());

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