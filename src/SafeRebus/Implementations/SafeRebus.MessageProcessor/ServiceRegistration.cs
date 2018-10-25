using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using SafeRebus.Abstractions;
using SafeRebus.RebusSteps;

namespace SafeRebus.MessageProcessor
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection.AddRebus((configure, serviceProvider) =>
            {
                var rabbitMqUtility = serviceProvider.GetService<IRabbitMqUtility>();
                return configure
                    .Logging(l => l.ColoredConsole(rabbitMqUtility.LogLevel))
                    .Options(optionsConfigure => optionsConfigure.HandleSafeRebusSteps(serviceProvider))
                    .Transport(t => t.UseRabbitMq(rabbitMqUtility.ConnectionString, rabbitMqUtility.InputQueue))
                    .Routing(r => r.TypeBased().MapFallback(rabbitMqUtility.OutputQueue));
            });
        }
    }
}