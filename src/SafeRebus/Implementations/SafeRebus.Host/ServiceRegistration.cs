using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SafeRebus.Host
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddHostedService<SafeRebusHost>();
        }
    }
}