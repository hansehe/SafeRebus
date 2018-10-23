using System;

namespace SafeRebus.Contracts.Requests
{
    public class DummyRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}