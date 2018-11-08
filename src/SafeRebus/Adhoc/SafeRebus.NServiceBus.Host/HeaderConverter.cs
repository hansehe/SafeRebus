using System;
using System.Collections.Generic;
using System.Globalization;
using NServiceBus;
using SafeRebus.Adapter.Utilities;
using SafeStandard;

namespace SafeRebus.NServiceBus.Host
{
    public static class HeaderConverter
    {
        public static bool ContainsSafeStandardHeaders(Dictionary<string, string> headers)
        {
            foreach (var headerKey in headers.Keys)
            {
                if (headerKey.Contains(SafeStandardHeaders.HeaderPrefix))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static void AppendSafeStandardHeaders(Dictionary<string, string> nServiceBusHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeStandardHeaders.ReplyToAddress] = nServiceBusHeaders[Headers.ReplyToAddress],
                nServiceBusHeaders.ContainsKey(Headers.ReplyToAddress));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeStandardHeaders.OriginatingAddress] = nServiceBusHeaders[Headers.OriginatingEndpoint],
                nServiceBusHeaders.ContainsKey(Headers.OriginatingEndpoint));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeStandardHeaders.CorrelationId] = nServiceBusHeaders[Headers.CorrelationId],
                nServiceBusHeaders.ContainsKey(Headers.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeStandardHeaders.ContentType] = nServiceBusHeaders[Headers.ContentType],
                nServiceBusHeaders.ContainsKey(Headers.ContentType));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeStandardHeaders.MessageId] = nServiceBusHeaders[Headers.MessageId],
                nServiceBusHeaders.ContainsKey(Headers.MessageId));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeStandardHeaders.MessageType] = nServiceBusHeaders[Headers.EnclosedMessageTypes],
                nServiceBusHeaders.ContainsKey(Headers.EnclosedMessageTypes));
            nServiceBusHeaders[SafeStandardHeaders.TimeSent] = DateTime.UtcNow.ToSafeHeaderValidString();
        }
        
        public static void AppendNServiceBusHeaders(Dictionary<string, string> safeStandardHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ReplyToAddress] = safeStandardHeaders[SafeStandardHeaders.ReplyToAddress],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.ReplyToAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.OriginatingEndpoint] = safeStandardHeaders[SafeStandardHeaders.OriginatingAddress],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.OriginatingAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.CorrelationId] = safeStandardHeaders[SafeStandardHeaders.CorrelationId],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ContentType] = safeStandardHeaders[SafeStandardHeaders.ContentType],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.ContentType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.MessageId] = safeStandardHeaders[SafeStandardHeaders.MessageId],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.MessageId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.EnclosedMessageTypes] = safeStandardHeaders[SafeStandardHeaders.MessageType],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.MessageType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.TimeSent] = DateTimeExtensions.ToWireFormattedString(safeStandardHeaders[SafeStandardHeaders.TimeSent].ToDatetimeFromSafeHeaderValidString()),
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.TimeSent));
        }
    }
}