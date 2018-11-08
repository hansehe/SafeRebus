using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Adapters.Abstractions.Adapters;

namespace SafeRebus.Adapter.SafeStandard
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<ISafeStandardAdapter, SafeStandardAdapter>();
        }
    }
}