using System;
using System.Collections.Generic;
using System.Globalization;
using NServiceBus;
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
            nServiceBusHeaders[SafeHeaders.ReplyToAddress] = nServiceBusHeaders[Headers.ReplyToAddress];
            nServiceBusHeaders[SafeHeaders.OriginatingAddress] = nServiceBusHeaders[Headers.OriginatingEndpoint];
            nServiceBusHeaders[SafeHeaders.CorrelationId] = nServiceBusHeaders[Headers.CorrelationId];
            nServiceBusHeaders[SafeHeaders.ContentType] = nServiceBusHeaders[Headers.ContentType];
            nServiceBusHeaders[SafeHeaders.MessageId] = nServiceBusHeaders[Headers.MessageId];
            nServiceBusHeaders[SafeHeaders.MessageType] = nServiceBusHeaders[Headers.EnclosedMessageTypes];
            nServiceBusHeaders[SafeHeaders.TimeSent] = DateTime.UtcNow.ToSafeHeaderValidString();
        }
        
        public static void AppendNServiceBusHeaders(Dictionary<string, string> safeStandardHeaders)
        {
            safeStandardHeaders[Headers.ReplyToAddress] = safeStandardHeaders[SafeHeaders.ReplyToAddress];
            safeStandardHeaders[Headers.OriginatingEndpoint] = safeStandardHeaders[SafeHeaders.OriginatingAddress];
            safeStandardHeaders[Headers.CorrelationId] = safeStandardHeaders[SafeHeaders.CorrelationId];
            safeStandardHeaders[Headers.ContentType] = safeStandardHeaders[SafeHeaders.ContentType];
            safeStandardHeaders[Headers.MessageId] = safeStandardHeaders[SafeHeaders.MessageId];
            safeStandardHeaders[Headers.EnclosedMessageTypes] = safeStandardHeaders[SafeHeaders.MessageType];
            safeStandardHeaders[Headers.TimeSent] = DateTimeExtensions.ToWireFormattedString(safeStandardHeaders[SafeHeaders.TimeSent].ToDatetimeFromSafeHeaderValidString());
        }
    }
}