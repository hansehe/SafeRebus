using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SafeRebus.Contracts.Requests;
using SafeRebus.Utilities;

namespace SafeRebus.MessageHandler
{
    public class DummyRequestMessageHandler : IHandleMessages<DummyRequest>
    {
        private readonly ILogger Logger;

        public DummyRequestMessageHandler(ILogger<DummyRequestMessageHandler> logger)
        {
            Logger = logger;
        }
        
        public Task Handle(DummyRequest message)
        {
            Logger.LogDebug($"Received message: {typeof(DummyRequest)}");
            Tools.MaybeThrowJokerException();
            return Task.CompletedTask;
        }
    }
}