using System.Collections.Generic;
using System.Linq;
using Rebus.Messages;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.NServiceBus
{
    public static class HeaderConverter
    {
        public static bool IsNServiceBusHeaders(Dictionary<string, string> headers)
        {
            return headers.ContainsKey(NServiceBusHeaders.ContentType);
        }
        
        public static Dictionary<string, string> AppendRebusHeaders(
            Dictionary<string, string> nServiceBusHeaders)
        {
            var rebusHeaders = new Dictionary<string, string>();
            nServiceBusHeaders.ToList().ForEach(x => rebusHeaders.Add(x.Key, x.Value));
            rebusHeaders[Headers.ReturnAddress] = nServiceBusHeaders[NServiceBusHeaders.ReplyToAddress];
            rebusHeaders[Headers.SenderAddress] = nServiceBusHeaders[NServiceBusHeaders.OriginatingEndpoint];
            rebusHeaders[Headers.CorrelationId] = nServiceBusHeaders[NServiceBusHeaders.CorrelationId];
            rebusHeaders[Headers.ContentType] = nServiceBusHeaders[NServiceBusHeaders.ContentType];
            rebusHeaders[Headers.Intent] = nServiceBusHeaders[NServiceBusHeaders.MessageIntent];
            rebusHeaders[Headers.MessageId] = nServiceBusHeaders[NServiceBusHeaders.MessageId];
            rebusHeaders[Headers.SentTime] = nServiceBusHeaders[NServiceBusHeaders.TimeSent];
            return rebusHeaders;
        }
        
        public static Dictionary<string, string> AppendNServiceBusHeaders(
            Dictionary<string, string> rebusHeaders)
        {
            var nServiceBusHeaders = new Dictionary<string, string>();
            rebusHeaders.ToList().ForEach(x => nServiceBusHeaders.Add(x.Key, x.Value));
            nServiceBusHeaders[NServiceBusHeaders.ReplyToAddress] = rebusHeaders[Headers.ReturnAddress];
            nServiceBusHeaders[NServiceBusHeaders.OriginatingEndpoint] = rebusHeaders[Headers.SenderAddress];
            nServiceBusHeaders[NServiceBusHeaders.CorrelationId] = rebusHeaders[Headers.CorrelationId];
            nServiceBusHeaders[NServiceBusHeaders.ContentType] = rebusHeaders[Headers.ContentType];
            nServiceBusHeaders[NServiceBusHeaders.MessageIntent] = rebusHeaders[Headers.Intent];
            nServiceBusHeaders[NServiceBusHeaders.MessageId] = rebusHeaders[Headers.MessageId];
            nServiceBusHeaders[NServiceBusHeaders.TimeSent] = rebusHeaders[Headers.SentTime];
            return nServiceBusHeaders;
        }
    }
}