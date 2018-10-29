using Microsoft.Extensions.DependencyInjection;

namespace SafeRebus.Outbox.Cleaner.Host
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