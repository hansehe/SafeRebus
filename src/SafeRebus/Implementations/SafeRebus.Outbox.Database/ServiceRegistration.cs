using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Database.Repositories;

namespace SafeRebus.Outbox.Database
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