using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.Outbox.Bus
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IOutboxBus, OutboxBus>();
        }
    }
}