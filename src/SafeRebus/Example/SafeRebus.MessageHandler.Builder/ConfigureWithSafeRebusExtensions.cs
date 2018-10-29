using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Extensions.Builder;

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
                .UseSafeRebusConfiguration();
        }
        
        public static IServiceCollection ConfigureWithSafeRebusOutboxCleanerForMessageHandler(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWithSafeRebusOutboxCleaner()
                .UseSafeRebusConfiguration()
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