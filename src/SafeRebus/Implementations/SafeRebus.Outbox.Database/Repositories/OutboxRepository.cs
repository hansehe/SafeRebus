using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Database.Outbox.Select;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.Database.Outbox.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IDbProvider DbProvider;

        public OutboxRepository(
            ILogger<OutboxRepository> logger,
            IConfiguration configuration,
            IDbProvider dbProvider)
        {
            Logger = logger;
            Configuration = configuration;
            DbProvider = dbProvider;
        }

        public async Task<bool> TryInsertMessageId(Guid id)
        {
            try
            {
                await InsertMessageId(id);
            }
            catch (Npgsql.PostgresException e)
            {
                Logger.LogWarning($"Message id already exists in database: {id.ToString()}");
                return false;
            }
            return true;
        }

        public Task InsertMessageId(Guid id)
        {
            Logger.LogDebug($"Inserting message id: {id.ToString()}");
            return Insert.InsertMessageId.Execute(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                id);
        }

        public async Task<bool> MessageIdExists(Guid id)
        {
            Logger.LogDebug($"Checking if message id exists: {id.ToString()}");
            var savedIds = await  SelectMessageIds.Select(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                id);
            return savedIds.Any(savedId => savedId == id);
        }

        public Task CleanOldMessageIds(TimeSpan tooOldThreshold)
        {
            Logger.LogDebug($"Deleting all old message ids older then: {tooOldThreshold.ToString()}");
            return Delete.DeleteMessageIdsFromTimestamp.Delete(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                tooOldThreshold);
        }
    }
}