using Microsoft.Extensions.DependencyInjection;

namespace SafeRebus.MessageHandler.Host
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