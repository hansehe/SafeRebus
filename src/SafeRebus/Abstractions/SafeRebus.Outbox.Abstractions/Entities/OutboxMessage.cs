using System;
using System.Collections.Generic;

namespace SafeRebus.Outbox.Abstractions.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public object Message { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string SendFunction { get; set; }
    }
}