using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SafeRebus.Builder
{
    public static class ConfigureWithRebusExtensions
    {
        public static IServiceCollection ConfigureWithSafeRebus(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .ConfigureWith(MessageProcessor.ServiceRegistration.Register)
                .ConfigureWith(MessageHandler.ServiceRegistration.Register)
                .ConfigureWith(Utilities.ServiceRegistration.Register)
                .ConfigureWith(Runner.ServiceRegistration.Register);
        }

        public static IServiceCollection UseDefaultRebusConfiguration(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Config.Config.GetConfigFilename)
                .Build());
        }

        private static IServiceCollection ConfigureWith(this IServiceCollection serviceCollection,
            Func<IServiceCollection, IServiceCollection> func)
        {
            return func.Invoke(serviceCollection);
        }
    }
}