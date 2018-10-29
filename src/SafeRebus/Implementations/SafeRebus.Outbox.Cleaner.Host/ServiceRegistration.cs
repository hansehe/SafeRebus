using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Outbox.Cleaner.Host;

namespace SafeRebus.Host.OutboxCleaner
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