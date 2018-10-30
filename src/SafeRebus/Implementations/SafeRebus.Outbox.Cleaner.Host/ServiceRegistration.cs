using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using SafeRebus.Abstractions;
using SafeRebus.ContainerAdapter;

namespace SafeRebus.Outbox.Cleaner.Host
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
                        .Transport(t => t.UseRabbitMqAsOneWayClient(rabbitMqUtility.ConnectionString));
                })
                .AddHostedService<OutboxCleanerHost>();
        }
    }
}