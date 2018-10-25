﻿using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Requests;
using SafeRebus.Contracts.Responses;
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
                .AddRebus((configure, serviceProvider) =>
                {
                    var rabbitMqUtility = serviceProvider.GetService<IRabbitMqUtility>();
                    return configure
                        .Logging(l => l.ColoredConsole(rabbitMqUtility.LogLevel))
                        .Options(optionsConfigure => optionsConfigure.HandleSafeRebusSteps(serviceProvider))
                        .Transport(t => t.UseRabbitMq(rabbitMqUtility.ConnectionString, rabbitMqUtility.InputQueue))
                        .Routing(r => r.TypeBased()
                            .Map<DummyRequest>(rabbitMqUtility.OutputQueue)
                            .Map<SafeRebusRequest>(rabbitMqUtility.OutputQueue)
                            .MapFallback(rabbitMqUtility.OutputQueue));
                });
        }
    }
}