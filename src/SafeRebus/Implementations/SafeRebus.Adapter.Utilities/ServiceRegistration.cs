using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Adapters.Abstractions;
using SafeRebus.Adapters.Abstractions.Adapters;

namespace SafeRebus.Adapter.Utilities
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IBodyConverter, BodyConverter>();
        }
    }
}