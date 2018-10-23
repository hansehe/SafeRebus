namespace SafeRebus.Abstractions
{
    public interface IMessageHandler : IMessageHandlerResolver
    {
        bool CanHandle<TMessage>(TMessage message);
    }
}