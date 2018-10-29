using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;

namespace SafeRebus.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbProvider, DbProvider>();
        }
    }
}