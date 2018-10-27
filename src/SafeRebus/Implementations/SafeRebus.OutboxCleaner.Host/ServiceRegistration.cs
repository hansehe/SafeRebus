using Microsoft.Extensions.DependencyInjection;

namespace SafeRebus.OutboxCleaner.Host
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddHostedService<OutboxCleanerHost>();
        }
    }
}