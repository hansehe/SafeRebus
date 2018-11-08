using Microsoft.Extensions.Logging;
using Rebus.Messages;
using SafeRebus.Adapters.Abstractions;
using SafeRebus.Adapters.Abstractions.Adapters;
using SafeStandard.Headers;

namespace SafeRebus.Adapter.SafeStandard
{
    public class SafeStandardAdapter : ISafeStandardAdapter
    {
        private readonly IBodyConverter BodyConverter;

        public SafeStandardAdapter(
            IBodyConverter bodyConverter)
        {
            BodyConverter = bodyConverter;
        }
        
        public bool IsUsableOnIncoming(TransportMessage transportMessage)
        {
            return HeaderConverter.ContainsSafeStandardHeaders(transportMessage.Headers);
        }

        public TransportMessage ConvertIncomingTransportMessage(TransportMessage incomingTransportMessage)
        {
            var safeStandardHeaders = incomingTransportMessage.Headers;
            var contentType = incomingTransportMessage.Headers[SafeHeaders.ContentType];
            var messageType = incomingTransportMessage.Headers[SafeHeaders.MessageType];
            var incomingBody = incomingTransportMessage.Body;

            var updatedTransportMessage = incomingTransportMessage;
            if (contentType != ContentTypes.RebusContentType && 
                BodyConverter.TryConvert(incomingBody, contentType, messageType, out var convertedBody))
            {
                safeStandardHeaders[SafeHeaders.ContentType] = ContentTypes.JsonContentType;
                updatedTransportMessage = new TransportMessage(safeStandardHeaders, convertedBody);
            }
            
            HeaderConverter.AppendRebusHeaders(safeStandardHeaders);
            return updatedTransportMessage;
        }

        public TransportMessage AppendAdapterSpecificHeaders(TransportMessage outgoingTransportMessage)
        {
            HeaderConverter.AppendSafeStandardHeaders(outgoingTransportMessage.Headers);
            return outgoingTransportMessage;
        }
    }
}