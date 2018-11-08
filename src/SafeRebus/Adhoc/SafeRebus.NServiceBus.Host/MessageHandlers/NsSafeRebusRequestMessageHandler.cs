using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.NServiceBus.Host.MessageHandlers
{
    public class NsSafeRebusRequestMessageHandler : IHandleMessages<SafeRebusRequest>
    {
        private readonly ILogger Logger;

        public NsSafeRebusRequestMessageHandler(
            ILogger<NsSafeRebusRequestMessageHandler> logger)
        {
            Logger = logger;
        }
        
        public Task Handle(SafeRebusRequest message, IMessageHandlerContext context)
        {
            Logger.LogDebug($"Received message: {typeof(SafeRebusRequest)}");
            return context.Reply(HandleRequest(message));
        }
        
        private static SafeRebusResponse HandleRequest(SafeRebusRequest request)
        {
            var response = new SafeRebusResponse
            {
                Id = request.Id,
                Response = "This is a response from the NServiceBus Host!"
            };
            return response;
        }
    }
}