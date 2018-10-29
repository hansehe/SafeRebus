using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.MessageHandler.Utilities;
using SafeRebus.Utilities;

namespace SafeRebus.MessageHandler
{
    public class SafeRebusResponseMessageHandler : IHandleMessages<SafeRebusResponse>
    {
        private readonly ILogger Logger;
        private readonly IResponseRepository ResponseRepository;

        public SafeRebusResponseMessageHandler(
            ILogger<SafeRebusResponseMessageHandler> logger,
            IResponseRepository responseRepository)
        {
            Logger = logger;
            ResponseRepository = responseRepository;
        }
        
        public async Task Handle(SafeRebusResponse message)
        {
            Logger.LogDebug($"Received message: {typeof(SafeRebusResponse)}");
            await HandleResponse(message);
        }
        
        private async Task HandleResponse(SafeRebusResponse response)
        {
            await ResponseRepository.InsertResponse(response);
            JokerException.MaybeThrowJokerException();
        }
    }
}