using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.Utilities;

namespace SafeRebus.OutboxCleaner.Host
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
            MainTask = Task.Run(async () => await Run(cancellationToken));
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
            var triggerCycle = Configuration.GetHostCleaningOutboxCyclePeriod();
            Logger.LogInformation($"Starting clean outbox which cleans the outbox every: {triggerCycle.ToString()}.");
            return Tools.TriggerEveryCycle(CleanOutbox, triggerCycle, cancellationToken);
        }
        
        private async Task CleanOutbox()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var threshold = Configuration.GetHostCleanOldMessageIdsFromOutboxTimeThreshold();
                Logger.LogInformation($"Cleaning outbox of message ids older then: {threshold.ToString()}.");
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await outboxRepository.CleanOldMessageIds(threshold);
                dbProvider.GetDbTransaction().Commit();
            }
        }
    }
}