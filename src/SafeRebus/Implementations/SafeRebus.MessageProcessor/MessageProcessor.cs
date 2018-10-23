﻿using System;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Requests;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.MessageProcessor
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageHandlerResolver MessageHandlerResolver;
        
        private readonly IRabbitMqUtility RabbitMqUtility;
        
        private readonly IServiceProvider ServiceProvider;
        
        public MessageProcessor(
            IMessageHandlerResolver messageHandlerResolver,
            IRabbitMqUtility rabbitMqUtility)
        {
            MessageHandlerResolver = messageHandlerResolver;
            RabbitMqUtility = rabbitMqUtility;
        }
        
        public IBus Init()
        {
            RabbitMqUtility.WaitForAvailableRabbitMq();
            return BuildBusHandler();
        }

        private IBus BuildBusHandler()
        {
            var activator = new BuiltinHandlerActivator();
            RegisterHandlers(activator);
            
            var busHandler = Configure
                .With(activator)
                .Logging(l => l.ColoredConsole(RabbitMqUtility.LogLevel))
                .Options(OptionConfiguration)
                .Transport(t => t.UseRabbitMq(RabbitMqUtility.ConnectionString, RabbitMqUtility.InputQueue))
                .Routing(r => r.TypeBased().MapFallback(RabbitMqUtility.OutputQueue))
                .Start();
            return busHandler;
        }

        private void RegisterHandlers(BuiltinHandlerActivator activator)
        {
            activator.Handle<SafeRebusRequest>(async (bus, message) => await MessageHandlerResolver.Handle(bus, message));
            activator.Handle<SafeRebusResponse>(async (bus, message) => await MessageHandlerResolver.Handle(bus, message));
        }
        
        private void OptionConfiguration(OptionsConfigurer optionsConfigurer)
        {
        }
    }
}