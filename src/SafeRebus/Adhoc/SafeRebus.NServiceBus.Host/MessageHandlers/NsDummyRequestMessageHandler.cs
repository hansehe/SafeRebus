using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SafeRebus.MessageHandler.Contracts.Requests;

namespace SafeRebus.NServiceBus.Host.MessageHandlers
{
    public class NsDummyRequestMessageHandler : IHandleMessages<DummyRequest>
    {
        private readonly ILogger Logger;

        public NsDummyRequestMessageHandler(ILogger<NsDummyRequestMessageHandler> logger)
        {
            Logger = logger;
        }
        
        public Task Handle(DummyRequest message, IMessageHandlerContext context)
        {
            Logger.LogDebug($"Received message: {typeof(DummyRequest)}");
            return context.Reply(message);
        }
    }
}