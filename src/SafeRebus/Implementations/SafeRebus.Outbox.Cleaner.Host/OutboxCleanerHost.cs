using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Config;
using SafeRebus.Utilities;

namespace SafeRebus.Outbox.Cleaner.Host
{
    public class OutboxCleanerHost : IHostedService
    {
        private readonly ILogger<OutboxCleanerHost> Logger;
        private readonly IConfiguration Configuration;
        private readonly IServiceProvider ServiceProvider;

        private Task MainTask;
        
        public OutboxCleanerHost(
            ILogger<OutboxCleanerHost> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            Configuration = configuration;
            ServiceProvider = serviceProvider;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting outbox cleaner host.");
            MainTask = Task.Run(() => Run(cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping outbox cleaner host.");
            MainTask.Wait(cancellationToken);
            return Task.CompletedTask;
        }

        private Task Run(CancellationToken cancellationToken)
        {
            var triggerCycle = Configuration.GetCleaningOutboxCyclePeriod();
            Logger.LogInformation($"Starting clean outbox which cleans the outbox every: {triggerCycle.ToString()}.");
            return Tools.TriggerEveryCycle(CleanOutbox, triggerCycle, cancellationToken);
        }
        
        private async Task CleanOutbox()
        {
            await CleanOutboxDuplicationFilter();
            await CleanOutboxMessages();
        }

        private async Task CleanOutboxDuplicationFilter()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var outboxCleaner = scope.ServiceProvider.GetService<IOutboxDuplicationFilterCleaner>();
                await outboxCleaner.CleanMessageIds();
            }
        }
        
        private async Task CleanOutboxMessages()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var outboxCleaner = scope.ServiceProvider.GetService<IOutboxMessageCleaner>();
                await outboxCleaner.CleanMessages();
            }
        }
    }
}