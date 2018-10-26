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
        private readonly IDbProvider DbProvider;

        public SafeRebusResponseMessageHandler(
            ILogger<SafeRebusResponseMessageHandler> logger,
            IResponseRepository responseRepository,
            IDbProvider dbProvider)
        {
            Logger = logger;
            ResponseRepository = responseRepository;
            DbProvider = dbProvider;
        }
        
        public async Task Handle(SafeRebusResponse message)
        {
            Logger.LogDebug($"Received message: {typeof(SafeRebusResponse)}");
            await HandleResponse(message);
            DbProvider.GetDbTransaction().Commit();
        }
        
        private async Task HandleResponse(SafeRebusResponse response)
        {
            await ResponseRepository.InsertResponse(response);
            JokerException.MaybeThrowJokerException();
        }
    }
}