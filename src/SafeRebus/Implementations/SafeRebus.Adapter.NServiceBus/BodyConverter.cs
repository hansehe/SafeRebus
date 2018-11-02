using Rebus.Messages;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.NServiceBus
{
    public static class BodyConverter
    {
        public static byte[] ConvertBodyFromNServiceBus(this IBodyConverter bodyConverter, TransportMessage incomingTransportMessage)
        {
            var contentType = incomingTransportMessage.Headers[NServiceBusHeaders.ContentType];
            var rebusBody = bodyConverter.Convert(incomingTransportMessage.Body, contentType);
            return rebusBody;
        }
    }
}