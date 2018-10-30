using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Config;

namespace SafeRebus.Outbox.Cleaner
{
    public class OutboxDuplicationFilterCleaner : IOutboxDuplicationFilterCleaner
    {
        private readonly ILogger Logger;
        private readonly IOutboxDuplicationFilterRepository OutboxDuplicationFilterRepository;
        private readonly IConfiguration Configuration;

        public OutboxDuplicationFilterCleaner(
            ILogger<OutboxDuplicationFilterCleaner> logger,
            IOutboxDuplicationFilterRepository outboxDuplicationFilterRepository,
            IConfiguration configuration)
        {
            Logger = logger;
            OutboxDuplicationFilterRepository = outboxDuplicationFilterRepository;
            Configuration = configuration;
        }
        
        public async Task CleanMessageIds()
        {
            var threshold = Configuration.GetCleanOldMessageIdsFromDuplicationFilterTimeThreshold();
            Logger.LogInformation($"Cleaning outbox duplication filter of message ids older then: {threshold.ToString()}.");
            await OutboxDuplicationFilterRepository.CleanOldMessageIds(threshold);
        }
    }
}