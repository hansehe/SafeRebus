using System;
using System.Threading.Tasks;

namespace SafeRebus.Outbox.Abstractions
{
    public interface IOutboxDuplicationFilterRepository
    {
        Task<bool> TryInsertMessageId(Guid id);
        Task InsertMessageId(Guid id);
        Task<bool> MessageIdExists(Guid id);
        Task CleanOldMessageIds(TimeSpan tooOldThreshold);
    }
}