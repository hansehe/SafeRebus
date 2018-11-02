using Rebus.Messages;

namespace SafeRebus.Adapters.Abstractions
{
    public interface IAdapter
    {
        bool IsUsableOnIncoming(TransportMessage transportMessage);

        TransportMessage ConvertIncomingTransportMessage(TransportMessage incomingTransportMessage);
        TransportMessage AppendAdapterSpecificHeaders(TransportMessage outgoingTransportMessage);
    }
}