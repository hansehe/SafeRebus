using Rebus.Bus;

namespace SafeRebus.Abstractions
{
    public interface IMessageProcessor
    {
        IBus Init();
    }
}