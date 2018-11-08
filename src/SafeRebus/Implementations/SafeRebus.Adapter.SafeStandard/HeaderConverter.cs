using System;
using System.Collections.Generic;
using System.Linq;
using Rebus.Messages;
using SafeRebus.Adapter.Utilities;
using SafeStandard;

namespace SafeRebus.Adapter.SafeStandard
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
        
        public static void AppendSafeStandardHeaders(Dictionary<string, string> rebusHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeStandardHeaders.OriginatingAddress] = rebusHeaders[Headers.ReturnAddress],
                rebusHeaders.ContainsKey(Headers.ReturnAddress));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeStandardHeaders.ReplyToAddress] = rebusHeaders[Headers.SenderAddress],
                rebusHeaders.ContainsKey(Headers.SenderAddress));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeStandardHeaders.CorrelationId] = rebusHeaders[Headers.CorrelationId],
                rebusHeaders.ContainsKey(Headers.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeStandardHeaders.ContentType] = rebusHeaders[Headers.ContentType],
                rebusHeaders.ContainsKey(Headers.ContentType));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeStandardHeaders.MessageId] = rebusHeaders[Headers.MessageId],
                rebusHeaders.ContainsKey(Headers.MessageId));
            AdapterUtilities.InvokeIfTrue(() => rebusHeaders[SafeStandardHeaders.MessageType] = rebusHeaders[Headers.Type],
                rebusHeaders.ContainsKey(Headers.Type));
            rebusHeaders[SafeStandardHeaders.TimeSent] = DateTime.UtcNow.ToSafeHeaderValidString();
        }
        
        public static void AppendRebusHeaders(Dictionary<string, string> safeStandardHeaders)
        {
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ReturnAddress] = safeStandardHeaders[SafeStandardHeaders.ReplyToAddress],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.ReplyToAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.SenderAddress] = safeStandardHeaders[SafeStandardHeaders.OriginatingAddress],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.OriginatingAddress));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.CorrelationId] = safeStandardHeaders[SafeStandardHeaders.CorrelationId],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.CorrelationId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.ContentType] = safeStandardHeaders[SafeStandardHeaders.ContentType],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.ContentType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.MessageId] = safeStandardHeaders[SafeStandardHeaders.MessageId],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.MessageId));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.Type] = safeStandardHeaders[SafeStandardHeaders.MessageType],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.MessageType));
            AdapterUtilities.InvokeIfTrue(() => safeStandardHeaders[Headers.SentTime] = safeStandardHeaders[SafeStandardHeaders.TimeSent],
                safeStandardHeaders.ContainsKey(SafeStandardHeaders.TimeSent));
        }
    }
}