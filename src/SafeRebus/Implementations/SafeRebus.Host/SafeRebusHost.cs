using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.Contracts.Requests;
using SafeRebus.Utilities;

namespace SafeRebus.Host
{
    public class SafeRebusHost : IHostedService, IDisposable
    {
        private CancellationToken CancellationToken;
        private bool ErrorStop = false;
        private Task AllTasks;
        private Timer OutboxCleanerTimer;
        
        private int RequestsPerCycle => Configuration.GetHostRequestsPerCycle();

        private readonly ILogger Logger;
        private readonly IBus Bus;
        private readonly IRabbitMqUtility RabbitMqUtility;
        private readonly IConfiguration Configuration;
        private readonly IServiceProvider ServiceProvider;
        private readonly IResponseRepository ResponseRepository;

        public SafeRebusHost(
            ILogger<SafeRebusHost> logger,
            IBus bus,
            IRabbitMqUtility rabbitMqUtility,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IResponseRepository responseRepository)
        {
            Logger = logger;
            Bus = bus;
            RabbitMqUtility = rabbitMqUtility;
            Configuration = configuration;
            ServiceProvider = serviceProvider;
            ResponseRepository = responseRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting host");
            CancellationToken = cancellationToken;
            
            var runningTasks = new List<Task>();
            
            RabbitMqUtility.PurgeInputQueue();
            
            var mainTask = Run();
            runningTasks.Add(mainTask);
            
            if (Configuration.HostShouldSendDummyRequests())
            {
                runningTasks.Add(SpamWithDummyRequests());
            }

            if (Configuration.HostShouldCleanOutbox())
            {
                StartCleanOutboxTimer();
            }

            AllTasks = Task.WhenAll(runningTasks.ToArray());
            return AllTasks;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping host");
            SafeRebusHostHelper.StopTimer(OutboxCleanerTimer);
            try
            {
                AllTasks.Wait(cancellationToken);
            }
            catch
            {
                Logger.LogError("Graceful shutdown failed!");
            }
            return Task.CompletedTask;
        }

        private Task Run()
        {
            Logger.LogInformation("Starting main host job - sending requests, and expects successful replies.");
            Logger.LogInformation($"Sending {RequestsPerCycle} requests during each cycle.");
            return RunUntilCancelled(async () =>
            {
                var requests = SafeRebusHostHelper.GetRequests(RequestsPerCycle);
                foreach (var request in requests)
                {
                    await Bus.Send(request);
                } 
                await Tools.WaitUntilSuccess(
                    () => ResponseRepository.AssertReceivedResponses(requests));
                Logger.LogInformation("Successfully received all requested responses!");
            });
        }

        private Task SpamWithDummyRequests()
        {
            Logger.LogInformation("Starting alternative job - spamming with dummy requests.");
            return RunUntilCancelled(async () => 
            {
                var request = new DummyRequest();
                await Bus.Send(request);
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            });
        }

        private void StartCleanOutboxTimer()
        {
            var period = Configuration.GetHostCleaningOutboxTimerPeriod();
            Logger.LogInformation($"Starting clean outbox timer which is triggered every: {period.ToString()} second.");
            OutboxCleanerTimer = new Timer(CleanOutboxTrigger, null, TimeSpan.Zero, 
                period);
        }

        private async void CleanOutboxTrigger(object state)
        {
            await ServiceProvider.CleanOutbox(Logger);
        }

        private async Task RunUntilCancelled(Func<Task> func)
        {
            while (!CancellationToken.IsCancellationRequested && !ErrorStop)
            {
                try
                {
                    await func.Invoke();
                }
                catch
                {
                    ErrorStop = true;
                    throw;
                }
            }
        }

        public void Dispose()
        {
            OutboxCleanerTimer?.Dispose();
            Bus?.Dispose();
        }
    }
}