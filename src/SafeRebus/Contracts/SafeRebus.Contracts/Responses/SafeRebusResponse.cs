using SafeRebus.Contracts.Requests;

namespace SafeRebus.Contracts.Responses
{
    public class SafeRebusResponse : SafeRebusRequest
    {
        public string Response { get; set; }
    }
}