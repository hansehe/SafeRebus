using NServiceBus;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.NServiceBus.Host.Contracts
{
    public class NServiceBusResponse : SafeRebusResponse, IMessage
    {  
    }
}