using NServiceBus;
using SafeRebus.MessageHandler.Contracts.Requests;

namespace SafeRebus.NServiceBus.Host.Contracts
{
    public class NServiceBusDummyRequest : DummyRequest, IMessage
    {
    }
}