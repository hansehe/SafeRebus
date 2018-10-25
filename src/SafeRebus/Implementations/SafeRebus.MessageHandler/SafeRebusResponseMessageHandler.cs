using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Responses;
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
        
        public Task Handle(SafeRebusResponse message)
        {
            Logger.LogDebug($"Received message: {typeof(SafeRebusResponse)}");
            return HandleResponse(message);
        }
        
        private async Task HandleResponse(SafeRebusResponse response)
        {
            await ResponseRepository.InsertResponse(response);
            Tools.MaybeThrowJokerException();
        }
    }
}