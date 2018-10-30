using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.Outbox.Cleaner
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IOutboxDuplicationFilterCleaner, OutboxDuplicationFilterCleaner>()
                .AddScoped<IOutboxMessageCleaner, OutboxMessageCleaner>();
        }
    }
}