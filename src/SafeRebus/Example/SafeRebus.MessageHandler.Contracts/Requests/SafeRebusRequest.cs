using System;

namespace SafeRebus.MessageHandler.Contracts.Requests
{
    public class SafeRebusRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}