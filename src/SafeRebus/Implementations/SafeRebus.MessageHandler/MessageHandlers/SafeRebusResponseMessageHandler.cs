using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Contracts;
using SafeRebus.Contracts.Requests;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.MessageHandler.MessageHandlers
{
    public class SafeRebusResponseMessageHandler : MessageHandlerBase<SafeRebusResponse>
    {
        public static Dictionary<Guid, SafeRebusResponse> ReceivedResponses { get; } = new Dictionary<Guid, SafeRebusResponse>();
        
        public override async Task Handle<TMessage>(IBus bus, TMessage message)
        {
            Console.WriteLine($"Received {typeof(SafeRebusResponse)}");
            HandleResponse(message as SafeRebusResponse);
        }

        private void HandleResponse(SafeRebusResponse response)
        {
            ReceivedResponses[response.Id] = response;
        }
    }
}