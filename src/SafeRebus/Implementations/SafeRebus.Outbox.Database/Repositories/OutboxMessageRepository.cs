using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Abstractions.Entities;

namespace SafeRebus.Outbox.Database.Repositories
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly ILogger<OutboxMessageRepository> Logger;
        private readonly IDbProvider DbProvider;
        private readonly IConfiguration Configuration;

        public OutboxMessageRepository(
            ILogger<OutboxMessageRepository> logger,
            IDbProvider dbProvider,
            IConfiguration configuration)
        {
            Logger = logger;
            DbProvider = dbProvider;
            Configuration = configuration;
        }
        
        public Task InsertOutboxMessage(OutboxMessage outboxMessage)
        {
            Logger.LogDebug($"Inserting outbox message with id: {outboxMessage.Id.ToString()}");
            return Insert.InsertOutboxMessage.Execute(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                outboxMessage);
        }

        public Task<IEnumerable<OutboxMessage>> SelectOutboxMessagesBeforeThreshold(TimeSpan threshold)
        {
            Logger.LogDebug($"Selecting outbox messages older then: {threshold.ToString()}");
            return Select.SelectOutboxMessages.Select(
                DbProvider.GetDbConnection(),
                Configuration,
                threshold);
        }

        public Task DeleteOutboxMessage(Guid id)
        {
            Logger.LogDebug($"Deleting outbox message with id: {id.ToString()}");
            return Delete.DeleteOutboxMessages.Delete(
                DbProvider.GetDbConnection(),
                Configuration,
                new[] {id});
        }
    }
}