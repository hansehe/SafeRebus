using System;
using System.Collections.Generic;
using Rebus.Messages;
using SafeStandard.Headers;

namespace SafeRebus.Adapter.SafeStandard
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
        
        public static void AppendSafeStandardHeaders(Dictionary<string, string> rebusHeaders)
        {
            rebusHeaders[SafeHeaders.OriginatingAddress] = rebusHeaders[Headers.ReturnAddress];
            rebusHeaders[SafeHeaders.ReplyToAddress] = rebusHeaders[Headers.SenderAddress];
            rebusHeaders[SafeHeaders.CorrelationId] = rebusHeaders[Headers.CorrelationId];
            rebusHeaders[SafeHeaders.ContentType] = rebusHeaders[Headers.ContentType];
            rebusHeaders[SafeHeaders.MessageId] = rebusHeaders[Headers.MessageId];
            rebusHeaders[SafeHeaders.MessageType] = rebusHeaders[Headers.Type];
            rebusHeaders[SafeHeaders.TimeSent] = DateTime.UtcNow.ToSafeHeaderValidString();
        }
        
        public static void AppendRebusHeaders(Dictionary<string, string> safeStandardHeaders)
        {
            safeStandardHeaders[Headers.ReturnAddress] = safeStandardHeaders[SafeHeaders.ReplyToAddress];
            safeStandardHeaders[Headers.SenderAddress] = safeStandardHeaders[SafeHeaders.OriginatingAddress];
            safeStandardHeaders[Headers.CorrelationId] = safeStandardHeaders[SafeHeaders.CorrelationId];
            safeStandardHeaders[Headers.ContentType] = safeStandardHeaders[SafeHeaders.ContentType];
            safeStandardHeaders[Headers.MessageId] = safeStandardHeaders[SafeHeaders.MessageId];
            safeStandardHeaders[Headers.Type] = safeStandardHeaders[SafeHeaders.MessageType];
            safeStandardHeaders[Headers.SentTime] = safeStandardHeaders[SafeHeaders.TimeSent];
        }
    }
}