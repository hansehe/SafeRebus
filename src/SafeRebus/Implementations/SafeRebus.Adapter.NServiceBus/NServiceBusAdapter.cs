using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Rebus.Messages;
using SafeRebus.Adapters.Abstractions;
using SafeRebus.Adapters.Abstractions.Adapters;

namespace SafeRebus.Adapter.NServiceBus
{
    public class NServiceBusAdapter : INServiceBusAdapter
    {
        private readonly ILogger Logger;
        private readonly IBodyConverter BodyConverter;

        public NServiceBusAdapter(
            ILogger<NServiceBusAdapter> logger,
            IBodyConverter bodyConverter)
        {
            Logger = logger;
            BodyConverter = bodyConverter;
        }
        
        public bool IsUsableOnIncoming(TransportMessage transportMessage)
        {
            return HeaderConverter.IsNServiceBusHeaders(transportMessage.Headers);
        }

        public TransportMessage ConvertIncomingTransportMessage(TransportMessage incomingTransportMessage)
        {
            var adapterHeaders = HeaderConverter.AppendRebusHeaders(incomingTransportMessage.Headers);
            var contentType = incomingTransportMessage.Headers[NServiceBusHeaders.ContentType];
            var adapterBody = incomingTransportMessage.Body;
            if (BodyConverter.TryConvert(adapterBody, contentType, out var convertedBody))
            {
                adapterHeaders[Headers.ContentType] = ContentTypes.RebusContentType;
                adapterHeaders[NServiceBusHeaders.ContentType] = ContentTypes.RebusContentType;
                adapterBody = convertedBody;
            }
            var updatedTransportMessage = new TransportMessage(adapterHeaders, adapterBody);
            return updatedTransportMessage;
        }

        public TransportMessage AppendAdapterSpecificHeaders(TransportMessage outgoingTransportMessage)
        {
            var adapterHeaders = HeaderConverter.AppendNServiceBusHeaders(outgoingTransportMessage.Headers);
            var updatedTransportMessage = new TransportMessage(adapterHeaders, outgoingTransportMessage.Body);
            return updatedTransportMessage;
        }
    }
}