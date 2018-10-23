using System.Threading.Tasks;
using Rebus.Bus;
using SafeRebus.Abstractions;

namespace SafeRebus.MessageHandler.MessageHandlers
{
    public abstract class MessageHandlerBase<THandler> : IMessageHandler
    {
        public abstract Task Handle<TMessage>(IBus bus, TMessage message);

        public bool CanHandle<TMessage>(TMessage message)
        {
            return message.GetType() == typeof(THandler);
        }
    }
}