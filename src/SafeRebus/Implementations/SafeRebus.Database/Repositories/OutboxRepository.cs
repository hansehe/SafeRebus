using System;
using System.Threading.Tasks;
using SafeRebus.Abstractions;

namespace SafeRebus.Database.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        public Task InsertMessageCorrelationId(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MessageCorrelationIdExists(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task CleanOldMessageCorrelationIds(TimeSpan threshold)
        {
            throw new NotImplementedException();
        }
    }
}