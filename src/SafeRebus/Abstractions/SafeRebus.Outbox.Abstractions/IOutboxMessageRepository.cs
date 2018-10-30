using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SafeRebus.Outbox.Abstractions.Entities;

namespace SafeRebus.Outbox.Abstractions
{
    public interface IOutboxMessageRepository
    {
        Task InsertOutboxMessage(OutboxMessage outboxMessage);
        Task<IEnumerable<OutboxMessage>> SelectOutboxMessagesBeforeThreshold(TimeSpan threshold);
        Task DeleteOutboxMessage(Guid id);
    }
}