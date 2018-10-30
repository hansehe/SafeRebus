using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using SafeRebus.Abstractions;
using SafeRebus.ContainerAdapter;
using SafeRebus.Extensions.Builder;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.RebusSteps;

namespace SafeRebus.MessageHandler
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IHandleMessages<DummyRequest>, DummyRequestMessageHandler>()
                .AddScoped<IHandleMessages<SafeRebusRequest>, SafeRebusRequestMessageHandler>()
                .AddScoped<IHandleMessages<SafeRebusResponse>, SafeRebusResponseMessageHandler>()
                .AddSafeRebus((configure, serviceProvider) =>
                {
                    var rabbitMqUtility = serviceProvider.GetService<IRabbitMqUtility>();
                    return configure
                        .Logging(l => l.ColoredConsole(rabbitMqUtility.LogLevel))
                        .Options(optionsConfigure => optionsConfigure.HandleSafeRebusSteps())
                        .Transport(t => t.UseRabbitMq(rabbitMqUtility.ConnectionString, rabbitMqUtility.InputQueue))
                        .Routing(r => r.TypeBased()
                            .Map<DummyRequest>(rabbitMqUtility.OutputQueue)
                            .Map<SafeRebusRequest>(rabbitMqUtility.OutputQueue)
                            .MapFallback(rabbitMqUtility.OutputQueue));
                });
        }
    }
}