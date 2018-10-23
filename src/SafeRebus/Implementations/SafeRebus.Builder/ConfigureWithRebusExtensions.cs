using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SafeRebus.Builder
{
    public static class ConfigureWithRebusExtensions
    {
        public static IServiceCollection ConfigureWithSafeRebus(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .ConfigureWith(MessageProcessor.ServiceRegistration.Register)
                .ConfigureWith(MessageHandler.ServiceRegistration.Register)
                .ConfigureWith(Utilities.ServiceRegistration.Register)
                .ConfigureWith(Runner.ServiceRegistration.Register)
                .UseDefaultRebusConfiguration(overrideConfig);
        }

        private static IServiceCollection UseDefaultRebusConfiguration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection.AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Config.Config.DefaultConfigFilename)
                .AddJsonFileIfTrue(Config.Config.DefaultConfigDockerFilename, () => Config.Config.InContainer)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build());
        }

        private static IServiceCollection ConfigureWith(this IServiceCollection serviceCollection,
            Func<IServiceCollection, IServiceCollection> func)
        {
            return func.Invoke(serviceCollection);
        }

        private static IConfigurationBuilder AddJsonFileIfTrue(this IConfigurationBuilder configurationBuilder,
            string path,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddJsonFile(path);
            }
            return configurationBuilder;
        }
        
        private static IConfigurationBuilder AddInMemoryIfTrue(this IConfigurationBuilder configurationBuilder,
            Dictionary<string, string> dict,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddInMemoryCollection(dict);
            }
            return configurationBuilder;
        }
    }
}