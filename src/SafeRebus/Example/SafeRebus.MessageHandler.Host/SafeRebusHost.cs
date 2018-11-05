using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Config;
using SafeRebus.MessageHandler.Utilities;

namespace SafeRebus.MessageHandler.Host
{
    public class SafeRebusHost : IHostedService, IDisposable
    {
        private Task MainTask;
        
        private int RequestsPerCycle => Configuration.GetHostRequestsPerCycle();

        private readonly ILogger Logger;
        private readonly IBus Bus;
        private readonly IRabbitMqUtility RabbitMqUtility;
        private readonly IConfiguration Configuration;
        private readonly IResponseRepository ResponseRepository;

        public SafeRebusHost(
            ILogger<SafeRebusHost> logger,
            IBus bus,
            IRabbitMqUtility rabbitMqUtility,
            IConfiguration configuration,
            IResponseRepository responseRepository)
        {
            Logger = logger;
            Bus = bus;
            RabbitMqUtility = rabbitMqUtility;
            Configuration = configuration;
            ResponseRepository = responseRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting SafeRebus host");
            
            RabbitMqUtility.PurgeInputQueue();

            MainTask = Task.Run(() => Run(cancellationToken));
            return MainTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping SafeRebus host");
            MainTask.Wait(cancellationToken);
            return Task.CompletedTask;
        }

        private Task Run(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting main host job - sending requests, and expects successful replies.");
            Logger.LogInformation($"Sending {RequestsPerCycle} requests during each cycle.");
            return SafeRebusHostHelper.RunUntilCancelled(async () =>
            {
                var requests = SafeRebusHostHelper.GetRequests(RequestsPerCycle);
                foreach (var request in requests)
                {
                    await Bus.Send(request);
                } 
                await Tools.WaitUntilSuccess(
                    () => ResponseRepository.AssertReceivedResponses(requests));
                Logger.LogInformation("Successfully received all requested responses!");
            }, cancellationToken);
        }

        public void Dispose()
        {
            Bus?.Dispose();
        }
    }
}