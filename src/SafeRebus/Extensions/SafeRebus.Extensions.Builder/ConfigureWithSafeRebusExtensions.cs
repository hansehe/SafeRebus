using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Outbox.Cleaner.Host;
using SafeRebus.Outbox.Migration;

namespace SafeRebus.Extensions.Builder
{
    public static class ConfigureWithSafeRebusExtensions
    {
        public static IServiceCollection ConfigureWithSafeRebus(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .ConfigureWith(Database.ServiceRegistration.Register)
                .ConfigureWith(Outbox.Database.ServiceRegistration.Register)
                .ConfigureWith(Outbox.Bus.ServiceRegistration.Register)
                .ConfigureWith(Utilities.ServiceRegistration.Register);
        }
        
        public static IServiceCollection ConfigureWithSafeRebusMigration(this IServiceCollection serviceCollection, 
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            return serviceCollection
                .ConfigureWithMigrations(
                    Migration.ServiceRegistration.Register,
                    IncludeAllMigrationAssemblies(assembliesWithMigrationModels))
                .ConfigureWith(Database.ServiceRegistration.Register)
                .ConfigureWith(Outbox.Database.ServiceRegistration.Register);
        }
        
        public static IServiceCollection ConfigureWithSafeRebusOutboxCleaner(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .ConfigureWith(ServiceRegistration.Register)
                .ConfigureWith(Database.ServiceRegistration.Register)
                .ConfigureWith(Outbox.Database.ServiceRegistration.Register);
        }

        public static IConfigurationBuilder AddDefaultSafeRebusConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder
                .AddJsonFile(Config.BaseConfig.DefaultConfigFilename)
                .AddJsonFileIfTrue(Config.BaseConfig.DefaultConfigDockerFilename, () => Config.BaseConfig.InContainer);
        }

        private static IEnumerable<Assembly> IncludeAllMigrationAssemblies(IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            var allAssembliesWithMigrationModels = new List<Assembly>
            {
                MigrationAssembly.GetMigrationAssembly
            };
            allAssembliesWithMigrationModels.AddRange(assembliesWithMigrationModels);
            return allAssembliesWithMigrationModels;
        }
    }
}