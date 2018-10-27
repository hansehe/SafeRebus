using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SafeRebus.Builder
{
    public static class ConfigureWithSafeRebusExtensions
    {
        public static IServiceCollection ConfigureWithSafeRebus(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWith(MessageHandler.ServiceRegistration.Register)
                .ConfigureWith(Database.ServiceRegistration.Register)
                .ConfigureWith(Utilities.ServiceRegistration.Register)
                .ConfigureWith(Host.ServiceRegistration.Register)
                .UseDefaultSafeRebusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }
        
        public static IServiceCollection ConfigureWithSafeRebusMigration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWith(Migration.ServiceRegistration.Register)
                .ConfigureWith(Database.ServiceRegistration.Register)
                .UseDefaultSafeRebusConfiguration(overrideConfig);
        }
        
        public static IServiceCollection ConfigureWithSafeRebusOutboxCleaner(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWith(OutboxCleaner.Host.ServiceRegistration.Register)
                .ConfigureWith(Database.ServiceRegistration.Register)
                .UseDefaultSafeRebusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }
        
        public static IServiceCollection ConfigureWithSafeRebusSpammer(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWith(MessageSpammer.Host.ServiceRegistration.Register)
                .ConfigureWith(Utilities.ServiceRegistration.Register)
                .UseDefaultSafeRebusConfiguration(overrideConfig)
                .UseDefaultLogging();
        }

        private static IServiceCollection UseDefaultSafeRebusConfiguration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection.AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .AddJsonFile(Config.BaseConfig.DefaultConfigFilename)
                .AddJsonFileIfTrue(Config.BaseConfig.DefaultConfigDockerFilename, () => Config.BaseConfig.InContainer)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build());
        }
    }
}