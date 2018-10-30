namespace SafeRebus.Outbox.Database
{
    public static class Columns
    {
        public const string Timestamp = "timestamp";
        public const string OutgoingMessageHeaders = "outgoing_message_headers";
        public const string OutgoingMessageObject = "outgoing_message_object";
        public const string OutgoingSendFunction = "outgoing_send_function";
    }
}