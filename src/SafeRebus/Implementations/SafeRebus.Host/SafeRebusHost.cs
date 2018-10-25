using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
    public class SafeRebusHost : IHostedService
    {
        private CancellationToken CancellationToken;
        private bool ErrorStop = false;
        private IList<Task> RunningTasks = new List<Task>();
        
        private int RequestsPerCycle => Configuration.GetHostRequestsPerCycle();

        private readonly ILogger Logger;
        private readonly IBus Bus;
        private readonly IConfiguration Configuration;
        private readonly IResponseRepository ResponseRepository;

        public SafeRebusHost(
            ILogger<SafeRebusHost> logger,
            IBus bus,
            IConfiguration configuration,
            IResponseRepository responseRepository)
        {
            Logger = logger;
            Bus = bus;
            Configuration = configuration;
            ResponseRepository = responseRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting host");
            CancellationToken = cancellationToken;
            
            var mainTask = Run();
            RunningTasks.Add(mainTask);
            if (Configuration.HostShouldSendDummyRequests())
            {
                RunningTasks.Add(SpamWithDummyRequests());
            }
            
            return Task.WhenAll(RunningTasks);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping host");
            foreach (var runningTask in RunningTasks.ToArray())
            {
                runningTask.Wait(cancellationToken);
            }
            return Task.CompletedTask;
        }

        private Task Run()
        {
            Logger.LogInformation("Starting main host job - sending requests, and expects successful replies.");
            Logger.LogInformation($"Sending {RequestsPerCycle} during each cycle.");
            return RunUntilCancelled(async () =>
            {
                var requests = GetRequests(RequestsPerCycle);
                foreach (var request in requests)
                {
                    await Bus.Send(request);
                } 
                await Tools.WaitUntilSuccess(
                    () => AssertReceivedResponses(requests));
                Logger.LogInformation("I'm all fine!");
            });
        }

        private Task SpamWithDummyRequests()
        {
            Logger.LogInformation("Starting alternative job - spamming with dummy requests.");
            return RunUntilCancelled(async () => 
            {
                var request = new DummyRequest();
                await Bus.Send(request);
            });
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

        private async Task AssertReceivedResponses(SafeRebusRequest[] requests)
        {
            foreach (var request in requests)
            {
                await ResponseRepository.SelectResponse(request.Id);
            }
        }

        private static SafeRebusRequest[] GetRequests(int nRequests)
        {
            var requests = new List<SafeRebusRequest>();
            for (var i = 0; i < nRequests; i++)
            {
                requests.Add(new SafeRebusRequest());
            }
            return requests.ToArray();
        }
    }
}