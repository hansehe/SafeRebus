using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Database.Repositories;

namespace SafeRebus.Database
{
    public class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbExecutor, DbExecutor>()
                .AddScoped<IDbProvider, DbProvider>()
                .AddScoped<IOutboxRepository, OutboxRepository>();
        }
    }
}