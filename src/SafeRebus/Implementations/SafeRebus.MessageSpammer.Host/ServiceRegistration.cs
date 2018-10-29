using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using SafeRebus.Abstractions;
using SafeRebus.ContainerAdapter;
using SafeRebus.Contracts.Requests;

namespace SafeRebus.MessageSpammer.Host
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSafeRebus((configure, serviceProvider) =>
                {
                    var rabbitMqUtility = serviceProvider.GetService<IRabbitMqUtility>();
                    return configure
                        .Logging(l => l.ColoredConsole(rabbitMqUtility.LogLevel))
                        .Transport(t => t.UseRabbitMqAsOneWayClient(rabbitMqUtility.ConnectionString))
                        .Routing(r => r.TypeBased()
                            .Map<DummyRequest>(rabbitMqUtility.OutputQueue)
                            .Map<SafeRebusRequest>(rabbitMqUtility.OutputQueue)
                            .MapFallback(rabbitMqUtility.OutputQueue));
                })
                .AddHostedService<MessageSpammerHost>();
        }
    }
}