using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using SafeRebus.Abstractions;

namespace SafeRebus.MessageProcessor
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IMessageProcessor, MessageProcessor>()
                .AddSingleton(serviceProvider => serviceProvider.GetService<IMessageProcessor>().Init());
        }
    }
}