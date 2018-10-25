using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Database.Delete;
using SafeRebus.Database.Insert;
using SafeRebus.Database.Select;

namespace SafeRebus.Database.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IDbExecutor DbExecutor;

        public OutboxRepository(
            ILogger<OutboxRepository> logger,
            IConfiguration configuration,
            IDbExecutor dbExecutor)
        {
            Logger = logger;
            Configuration = configuration;
            DbExecutor = dbExecutor;
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
            return DbExecutor.ExecuteInTransactionAsync(dbConnection =>
                InsertCorrelationId.Execute(dbConnection, Configuration, id));
        }

        public async Task<bool> MessageIdExists(Guid id)
        {
            var savedIds = await DbExecutor.SelectInTransactionAsync(dbConnection =>
                SelectCorrelationIds.Select(dbConnection, Configuration, id));
            return savedIds.Any(savedId => savedId == id);
        }

        public Task CleanOldMessageIds(TimeSpan tooOldThreshold)
        {
            var dateTimeThreshold = DateTime.UtcNow - tooOldThreshold;
            return DbExecutor.ExecuteInTransactionAsync(dbConnection =>
                DeleteCorrelationIdsFromTimestamp.Delete(dbConnection, Configuration, dateTimeThreshold));
        }
    }
}