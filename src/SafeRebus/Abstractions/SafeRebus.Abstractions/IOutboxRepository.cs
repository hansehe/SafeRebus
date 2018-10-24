using System;
using System.Threading.Tasks;

namespace SafeRebus.Abstractions
{
    public interface IOutboxRepository
    {
        Task InsertMessageCorrelationId(Guid id);
        Task<bool> MessageCorrelationIdExists(Guid id);
        Task CleanOldMessageCorrelationIds(TimeSpan threshold);
    }
}