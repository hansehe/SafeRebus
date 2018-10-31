using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SafeRebus.Abstractions;
using SafeRebus.Extensions.Builder;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.NServiceBus.Host.Contracts;
using SafeRebus.NServiceBus.Host.MessageHandlers;
using SafeRebus.Utilities;

namespace SafeRebus.NServiceBus.Host
{
    public static class ServiceRegistration
    {
        public static IServiceCollection ConfigureWithNServiceBusHost(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IHandleMessages<NServiceBusDummyRequest>, DummyRequestMessageHandler>()
                .AddScoped<IHandleMessages<SafeRebusRequest>, SafeRebusRequestMessageHandler>()
                .AddScoped<IRabbitMqUtility, RabbitMqUtility>()
                .AddHostedService<NServiceBusHost>()
                .UseNServiceBusConfiguration()
                .UseDefaultLogging();
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