using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SafeRebus.Extensions.Builder
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseDefaultLogging(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddLogging(configure => configure
                    .AddConsole());
        }

        public static IServiceCollection ConfigureWith(this IServiceCollection serviceCollection,
            Func<IServiceCollection, IServiceCollection> func)
        {
            return func.Invoke(serviceCollection);
        }
        
        public static IServiceCollection ConfigureWithMigrations(this IServiceCollection serviceCollection, 
            Func<IServiceCollection, IEnumerable<Assembly>, IServiceCollection> func,
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            return func.Invoke(serviceCollection, assembliesWithMigrationModels);
        }

        public static IConfigurationBuilder AddJsonFileIfTrue(this IConfigurationBuilder configurationBuilder,
            string path,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddJsonFile(path);
            }
            return configurationBuilder;
        }
        
        public static IConfigurationBuilder AddInMemoryIfTrue(this IConfigurationBuilder configurationBuilder,
            Dictionary<string, string> dict,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddInMemoryCollection(dict);
            }
            return configurationBuilder;
        }
        
        public static IConfigurationBuilder AddConfigurationIfTrue(this IConfigurationBuilder configurationBuilder,
            IConfiguration configuration,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddConfiguration(configuration);
            }
            return configurationBuilder;
        }
        
    }
}