namespace SafeStandard
{
    public static class SafeStandardHeaders
    {
        public const string HeaderPrefix = "SafeStandardHeaders";
        
        public static readonly string ContentType = $"{HeaderPrefix}.ContentType"; // ~= Example: application/json;charset=utf8
        public static readonly string CorrelationId = $"{HeaderPrefix}.CorrelationId"; // ~= <GUID> (consistent for all correlating messages)
        public static readonly string MessageType = $"{HeaderPrefix}.MessageType"; // ~= <CONTRACT_NAMESPACE>.<CONTRACT_TYPE>
        public static readonly string MessageId = $"{HeaderPrefix}.MessageId"; // ~= <GUID> (unique for every message)
        public static readonly string OriginatingAddress = $"{HeaderPrefix}.OriginatingAddress"; // ~= Originating bus address
        public static readonly string ReplyToAddress = $"{HeaderPrefix}.ReplyToAddress"; // ~= Which bus address to send replies
        public static readonly string TimeSent = $"{HeaderPrefix}.TimeSent"; // ~= Timestamp at what time the message was sent
    }
}