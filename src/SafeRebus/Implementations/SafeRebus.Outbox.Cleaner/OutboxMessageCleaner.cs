using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Config;

namespace SafeRebus.Outbox.Cleaner
{
    public class OutboxMessageCleaner : IOutboxMessageCleaner
    {
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IOutboxMessageRepository OutboxMessageRepository;
        private readonly IOutboxBus Bus;

        public OutboxMessageCleaner(
            ILogger<OutboxMessageCleaner> logger,
            IConfiguration configuration,
            IOutboxMessageRepository outboxMessageRepository,
            IOutboxBus bus)
        {
            Logger = logger;
            Configuration = configuration;
            OutboxMessageRepository = outboxMessageRepository;
            Bus = bus;
        }
        
        public async Task CleanMessages()
        {
            var threshold = Configuration.GetCleanOutboxMessagesTimeThreshold();
            Logger.LogInformation($"Cleaning outbox of outgoing messages with ids older then: {threshold.ToString()}.");
            var outboxMessages = await OutboxMessageRepository.SelectOutboxMessagesBeforeThreshold(threshold);
            foreach (var outboxMessage in outboxMessages)
            {
                try
                {
                    await Bus.ResendOutboxMessage(outboxMessage);
                    await OutboxMessageRepository.DeleteOutboxMessage(outboxMessage.Id);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Could not resend outbox message with id: {outboxMessage.Id}. \r\nFollowing error occured: {e.Message}");
                }
            }
        }
    }
}