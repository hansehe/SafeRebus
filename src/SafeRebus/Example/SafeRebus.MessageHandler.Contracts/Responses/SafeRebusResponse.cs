using System;

namespace SafeRebus.MessageHandler.Contracts.Responses
{
    public class SafeRebusResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Response { get; set; } = "Some random response";
    }
}