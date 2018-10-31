using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SafeRebus.NServiceBus.Host.Contracts;

namespace SafeRebus.NServiceBus.Host.MessageHandlers
{
    public class DummyRequestMessageHandler : IHandleMessages<NServiceBusDummyRequest>
    {
        private readonly ILogger Logger;

        public DummyRequestMessageHandler(ILogger<DummyRequestMessageHandler> logger)
        {
            Logger = logger;
        }
        
        public Task Handle(NServiceBusDummyRequest message, IMessageHandlerContext context)
        {
            Logger.LogDebug($"Received message: {typeof(NServiceBusDummyRequest)}");
            return context.Reply(message);
        }
    }
}