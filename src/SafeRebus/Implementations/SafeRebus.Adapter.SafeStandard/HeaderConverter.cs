using System;
using System.Collections.Generic;
using System.Linq;
using Rebus.Messages;
using SafeRebus.Adapter.Utilities;
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
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeHeaders.OriginatingAddress] = rebusHeaders[Headers.ReturnAddress],
                rebusHeaders.ContainsKey(Headers.ReturnAddress));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeHeaders.ReplyToAddress] = rebusHeaders[Headers.SenderAddress],
                rebusHeaders.ContainsKey(Headers.SenderAddress));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeHeaders.CorrelationId] = rebusHeaders[Headers.CorrelationId],
                rebusHeaders.ContainsKey(Headers.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeHeaders.ContentType] = rebusHeaders[Headers.ContentType],
                rebusHeaders.ContainsKey(Headers.ContentType));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeHeaders.MessageId] = rebusHeaders[Headers.MessageId],
                rebusHeaders.ContainsKey(Headers.MessageId));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeHeaders.MessageType] = rebusHeaders[Headers.Type],
                rebusHeaders.ContainsKey(Headers.Type));
            rebusHeaders[SafeHeaders.TimeSent] = DateTime.UtcNow.ToSafeHeaderValidString();
        }
        
        public static void AppendRebusHeaders(Dictionary<string, string> safeStandardHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ReturnAddress] = safeStandardHeaders[SafeHeaders.ReplyToAddress],
                safeStandardHeaders.ContainsKey(SafeHeaders.ReplyToAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.SenderAddress] = safeStandardHeaders[SafeHeaders.OriginatingAddress],
                safeStandardHeaders.ContainsKey(SafeHeaders.OriginatingAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.CorrelationId] = safeStandardHeaders[SafeHeaders.CorrelationId],
                safeStandardHeaders.ContainsKey(SafeHeaders.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ContentType] = safeStandardHeaders[SafeHeaders.ContentType],
                safeStandardHeaders.ContainsKey(SafeHeaders.ContentType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.MessageId] = safeStandardHeaders[SafeHeaders.MessageId],
                safeStandardHeaders.ContainsKey(SafeHeaders.MessageId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.Type] = safeStandardHeaders[SafeHeaders.MessageType],
                safeStandardHeaders.ContainsKey(SafeHeaders.MessageType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.SentTime] = safeStandardHeaders[SafeHeaders.TimeSent],
                safeStandardHeaders.ContainsKey(SafeHeaders.TimeSent));
        }
    }
}