using System;
using System.Threading.Tasks;

namespace SafeRebus.Abstractions
{
    public interface IOutboxRepository
    {
        Task<bool> TryInsertMessageId(Guid id);
        Task InsertMessageId(Guid id);
        Task<bool> MessageIdExists(Guid id);
        Task CleanOldMessageIds(TimeSpan tooOldThreshold);
    }
}