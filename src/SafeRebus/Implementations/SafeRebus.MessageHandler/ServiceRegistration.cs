using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Responses;
using SafeRebus.MessageHandler.MessageHandlers;

namespace SafeRebus.MessageHandler
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IMessageHandlerResolver, MessageHandlerResolver>()
                .AddScoped<IMessageHandler, SafeRebusRequestMessageHandler>()
                .AddScoped<IMessageHandler, SafeRebusResponseMessageHandler>()
                .AddScoped<IMessageHandler, DummyRequestMessageHandler>();
        }
    }
}