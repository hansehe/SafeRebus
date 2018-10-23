using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;

namespace SafeRebus.Utilities
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IRabbitMqUtility, RabbitMqUtility>();
        }
    }
}