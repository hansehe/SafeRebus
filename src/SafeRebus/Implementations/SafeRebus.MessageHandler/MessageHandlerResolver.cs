using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus;
using SafeRebus.Abstractions;

namespace SafeRebus.MessageHandler
{
    public class MessageHandlerResolver : IMessageHandlerResolver
    {
        private readonly IEnumerable<IMessageHandler> MessageHandlers;

        public MessageHandlerResolver(
            IEnumerable<IMessageHandler> messageHandlers)
        {
            MessageHandlers = messageHandlers;
        }
        
        public Task Handle<TMessage>(IBus bus, TMessage message)
        {
            foreach (var messageHandler in MessageHandlers)
            {
                if (messageHandler.CanHandle(message))
                {
                    return messageHandler.Handle(bus, message);
                }
            }
            var errorMsg = $"Could not find a suitable message handler for message type: {message.GetType()}";
            throw new Exception(errorMsg);
        }
    }
}