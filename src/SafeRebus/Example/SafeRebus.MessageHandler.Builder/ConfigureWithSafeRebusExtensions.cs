using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using SafeRebus.Abstractions;
using SafeRebus.Extensions.Builder;
using SafeRebus.ContainerAdapter;
using SafeRebus.MessageHandler.Contracts.Requests;

namespace SafeRebus.MessageHandler.Builder
{
    public static class ConfigureWithSafeRebusExtensions
    {
        public static IServiceCollection ConfigureWithSafeRebusMessageHandler(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWithSafeRebus()
                .ConfigureWith(ServiceRegistration.Register)
                .ConfigureWith(Database.ServiceRegistration.Register)
                .ConfigureWith(Host.ServiceRegistration.Register)
                .UseSafeRebusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }
        
        public static IServiceCollection ConfigureWithSafeRebusMessageHandlerMigration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWithSafeRebusMigration(GetMigrationAssemblies())
                .UseSafeRebusConfiguration(overrideConfig);
        }
        
        public static IServiceCollection ConfigureWithSafeRebusOutboxCleanerHost(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return Extensions.Builder.ConfigureWithSafeRebusExtensions
                .ConfigureWithSafeRebusOutboxCleanerHost(serviceCollection)
                .AddSafeRebus((configure, serviceProvider) =>
                {
                    var rabbitMqUtility = serviceProvider.GetService<IRabbitMqUtility>();
                    return configure
                        .Logging(l => l.ColoredConsole(rabbitMqUtility.LogLevel))
                        .Transport(t => t.UseRabbitMqAsOneWayClient(rabbitMqUtility.ConnectionString))
                        .Routing(r => r.TypeBased()
                        .MapFallback(rabbitMqUtility.OutputQueue));
                })
                .UseSafeRebusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }
        
        public static IServiceCollection ConfigureWithSafeRebusMessageSpammer(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWithSafeRebus()
                .ConfigureWith(MessageSpammer.Host.ServiceRegistration.Register)
                .UseSafeRebusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }

        private static IServiceCollection UseSafeRebusConfiguration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection.AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .AddDefaultSafeRebusConfiguration()
                .AddJsonFile(Config.MessageHandlerConfig.DefaultMessageHandlerConfigFilename)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build());
        }

        private static IEnumerable<Assembly> GetMigrationAssemblies()
        {
            return new[]
            {
                Migration.MigrationAssembly.GetMigrationAssembly
            };
        }
    }
}