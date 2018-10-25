using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.MessageHandler
{
    public class SafeRebusResponseMessageHandler : IHandleMessages<SafeRebusResponse>
    {
        private readonly ILogger Logger;
        public static Dictionary<Guid, SafeRebusResponse> ReceivedResponses { get; } = new Dictionary<Guid, SafeRebusResponse>();

        public SafeRebusResponseMessageHandler(
            ILogger<SafeRebusResponseMessageHandler> logger)
        {
            Logger = logger;
        }
        
        public Task Handle(SafeRebusResponse message)
        {
            Logger.LogDebug($"Received {typeof(SafeRebusResponse)}");
            HandleResponse(message);
            return Task.CompletedTask;
        }
        
        private static void HandleResponse(SafeRebusResponse response)
        {
            ReceivedResponses[response.Id] = response;
        }
    }
}