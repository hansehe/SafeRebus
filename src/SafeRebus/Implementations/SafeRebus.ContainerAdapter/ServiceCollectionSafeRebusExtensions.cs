using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Pipeline;

namespace SafeRebus.ContainerAdapter
{
    public static class ServiceCollectionSafeRebusExtensions
    {
        public static IServiceCollection AddSafeRebus(this IServiceCollection services, Func<RebusConfigurer, RebusConfigurer> configureRebus)
        {
            return AddSafeRebus(services, (c, p) => configureRebus(c));
        }
        
        public static IServiceCollection AddSafeRebus(this IServiceCollection services, Func<RebusConfigurer, IServiceProvider, RebusConfigurer> configureRebus)
        {
            if (configureRebus == null)
            {
                throw new ArgumentNullException(nameof(configureRebus));
            }

            var messageBusRegistration = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IBus));

            if (messageBusRegistration != null)
            {
                throw new InvalidOperationException("Rebus has already been configured.");
            }

            services.AddTransient(s => MessageContext.Current);
            services.AddTransient(s => s.GetService<IBus>().Advanced.SyncBus);

            services.AddSingleton<IContainerAdapter, SafeRebusContainerAdapter>();
            services.AddSingleton(provider =>
            {
                var configurer = Configure.With(provider.GetRequiredService<IContainerAdapter>());
                configureRebus(configurer, provider);

                return configurer.Start();
            });

            return services;
        }
    }
}