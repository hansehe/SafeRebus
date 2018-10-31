using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Messages;
using Rebus.Transport;
using SafeRebus.Outbox.Abstractions.Entities;

namespace SafeRebus.Outbox.Abstractions
{
    public interface IOutboxBus : IBus
    {
        Task Commit();
        Task BeginTransaction(TransportMessage transportMessage);
        Task ResendOutboxMessage(OutboxMessage outboxMessage);
    }
}