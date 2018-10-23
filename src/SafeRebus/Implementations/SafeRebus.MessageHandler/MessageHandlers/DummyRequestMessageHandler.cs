using System;
using System.Threading.Tasks;
using Rebus.Bus;
using SafeRebus.Contracts.Requests;

namespace SafeRebus.MessageHandler.MessageHandlers
{
    public class DummyRequestMessageHandler : MessageHandlerBase<DummyRequest>
    {
        public override Task Handle<TMessage>(IBus bus, TMessage message)
        {
            Console.WriteLine($"Received {typeof(DummyRequest)}");
            HandleRequest(message as DummyRequest);
            return Task.CompletedTask;
        }

        private static void HandleRequest(DummyRequest request)
        {
            
        }
    }
}