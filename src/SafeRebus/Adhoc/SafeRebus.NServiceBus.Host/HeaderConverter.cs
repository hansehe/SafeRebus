using System;
using System.Collections.Generic;
using System.Globalization;
using NServiceBus;
using SafeRebus.Adapter.Utilities;
using SafeStandard.Headers;

namespace SafeRebus.NServiceBus.Host
{
    public static class HeaderConverter
    {
        public static bool ContainsSafeStandardHeaders(Dictionary<string, string> headers)
        {
            foreach (var headerKey in headers.Keys)
            {
                if (headerKey.Contains(SafeHeaders.HeaderPrefix))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static void AppendSafeStandardHeaders(Dictionary<string, string> nServiceBusHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeHeaders.ReplyToAddress] = nServiceBusHeaders[Headers.ReplyToAddress],
                nServiceBusHeaders.ContainsKey(Headers.ReplyToAddress));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeHeaders.OriginatingAddress] = nServiceBusHeaders[Headers.OriginatingEndpoint],
                nServiceBusHeaders.ContainsKey(Headers.OriginatingEndpoint));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeHeaders.CorrelationId] = nServiceBusHeaders[Headers.CorrelationId],
                nServiceBusHeaders.ContainsKey(Headers.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeHeaders.ContentType] = nServiceBusHeaders[Headers.ContentType],
                nServiceBusHeaders.ContainsKey(Headers.ContentType));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeHeaders.MessageId] = nServiceBusHeaders[Headers.MessageId],
                nServiceBusHeaders.ContainsKey(Headers.MessageId));
            AdapterUtilities.InvokeIfTrue(() => nServiceBusHeaders[SafeHeaders.MessageType] = nServiceBusHeaders[Headers.EnclosedMessageTypes],
                nServiceBusHeaders.ContainsKey(Headers.EnclosedMessageTypes));
            nServiceBusHeaders[SafeHeaders.TimeSent] = DateTime.UtcNow.ToSafeHeaderValidString();
        }
        
        public static void AppendNServiceBusHeaders(Dictionary<string, string> safeStandardHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ReplyToAddress] = safeStandardHeaders[SafeHeaders.ReplyToAddress],
                safeStandardHeaders.ContainsKey(SafeHeaders.ReplyToAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.OriginatingEndpoint] = safeStandardHeaders[SafeHeaders.OriginatingAddress],
                safeStandardHeaders.ContainsKey(SafeHeaders.OriginatingAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.CorrelationId] = safeStandardHeaders[SafeHeaders.CorrelationId],
                safeStandardHeaders.ContainsKey(SafeHeaders.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ContentType] = safeStandardHeaders[SafeHeaders.ContentType],
                safeStandardHeaders.ContainsKey(SafeHeaders.ContentType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.MessageId] = safeStandardHeaders[SafeHeaders.MessageId],
                safeStandardHeaders.ContainsKey(SafeHeaders.MessageId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.EnclosedMessageTypes] = safeStandardHeaders[SafeHeaders.MessageType],
                safeStandardHeaders.ContainsKey(SafeHeaders.MessageType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.TimeSent] = DateTimeExtensions.ToWireFormattedString(safeStandardHeaders[SafeHeaders.TimeSent].ToDatetimeFromSafeHeaderValidString()),
                safeStandardHeaders.ContainsKey(SafeHeaders.TimeSent));
        }
    }
}