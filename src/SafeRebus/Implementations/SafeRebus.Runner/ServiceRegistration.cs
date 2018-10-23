using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;

namespace SafeRebus.Runner
{
    public class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IRebusRunner, SafeRebusRunner>();
        }
    }
}