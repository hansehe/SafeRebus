using System.Threading.Tasks;
using Rebus.Bus;

namespace SafeRebus.Abstractions
{
    public interface IMessageHandlerResolver
    {
        Task Handle<TMessage>(IBus bus, TMessage message);
    }
}