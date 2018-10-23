using System;

namespace SafeRebus.Contracts.Requests
{
    public class SafeRebusRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public RequestEnum RequestEnum { get; set; }
    }
}