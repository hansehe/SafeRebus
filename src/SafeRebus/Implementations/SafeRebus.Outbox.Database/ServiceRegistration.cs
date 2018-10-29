using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Database.Outbox.Repositories;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.Database.Outbox
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IOutboxRepository, OutboxRepository>();
        }
    }
}