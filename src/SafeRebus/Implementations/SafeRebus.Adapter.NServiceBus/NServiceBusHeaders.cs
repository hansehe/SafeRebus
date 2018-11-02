namespace SafeRebus.Adapter.NServiceBus
{
    public static class NServiceBusHeaders
    {
        public const string HeaderPrefix = "NServiceBus";
        
        public static string ContentType = $"{HeaderPrefix}.ContentType";
        public static string ConversationId = $"{HeaderPrefix}.ConversationId";
        public static string CorrelationId = $"{HeaderPrefix}.CorrelationId";
        public static string EnclosedMessageTypes = $"{HeaderPrefix}.EnclosedMessageTypes";
        public static string MessageId = $"{HeaderPrefix}.MessageId";
        public static string MessageIntent = $"{HeaderPrefix}.MessageIntent";
        public static string OriginatingEndpoint = $"{HeaderPrefix}.OriginatingEndpoint";
        public static string OriginatingMachine = $"{HeaderPrefix}.OriginatingMachine";
        public static string ReplyToAddress = $"{HeaderPrefix}.ReplyToAddress";
        public static string TimeSent = $"{HeaderPrefix}.TimeSent";
    }
}