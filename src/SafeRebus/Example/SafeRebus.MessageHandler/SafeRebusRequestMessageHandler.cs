using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.MessageHandler.Utilities;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.MessageHandler
{
    public class SafeRebusRequestMessageHandler : IHandleMessages<SafeRebusRequest>
    {
        private readonly ILogger Logger;
        private readonly IBus Bus;

        public SafeRebusRequestMessageHandler(
            ILogger<SafeRebusRequestMessageHandler> logger,
            IOutboxBus bus)
        {
            Logger = logger;
            Bus = bus;
        }

        public async Task Handle(SafeRebusRequest message)
        {
            Logger.LogDebug($"Received message: {typeof(SafeRebusRequest)}");
            var response = HandleRequest(message);
            await Bus.Reply(response);
            JokerException.MaybeThrowJokerException();
        }

        private static SafeRebusResponse HandleRequest(SafeRebusRequest request)
        {
            var response = new SafeRebusResponse
            {
                Id = request.Id,
                Response = GetRandomResponse()
            };
            return response;
        }

        private static string GetRandomResponse()
        {
            var random = new Random();
            return $"This should be a totally random slogan.., soo here is a random number {random.Next()}";
        }
    }
}