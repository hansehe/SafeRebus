using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SafeRebus.Abstractions;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.NServiceBus.Host.MessageHandlers
{
    public class NsSafeRebusResponseMessageHandler : IHandleMessages<SafeRebusResponse>
    {
        private readonly ILogger Logger;
        private readonly IResponseRepository ResponseRepository;
        private readonly IDbProvider DbProvider;

        public NsSafeRebusResponseMessageHandler(
            ILogger<NsSafeRebusResponseMessageHandler> logger,
            IResponseRepository responseRepository,
            IDbProvider dbProvider)
        {
            Logger = logger;
            ResponseRepository = responseRepository;
            DbProvider = dbProvider;
        }
        
        public Task Handle(SafeRebusResponse message, IMessageHandlerContext context)
        {
            Logger.LogDebug($"Received message: {typeof(SafeRebusResponse)}");
            return context.Reply(HandleRequest(message));
        }
        
        private async Task HandleRequest(SafeRebusResponse response)
        {
            await ResponseRepository.InsertResponse(response);
            DbProvider.GetDbTransaction().Commit();
        }
    }
}