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
    public class SafeRebusHost : IHostedService
    {
        private CancellationToken CancellationToken;
        private IList<Task> RunningTasks = new List<Task>();
        
        private long PauseBetweenRequestsMs => Configuration.GetHostPauseBetweenRequestsMs();
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
            CancellationToken = cancellationToken;
            var mainTask = Run();
            RunningTasks.Add(mainTask);
            if (Configuration.HostShouldSendDummyRequests())
            {
                RunningTasks.Add(SpamWithDummyRequests());
            }
            return mainTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping host");
            foreach (var runningTask in RunningTasks)
            {
                runningTask.Wait(cancellationToken);
            }
            return Task.CompletedTask;
        }

        private Task Run()
        {
            Logger.LogInformation("Starting main host job - sending requests, and expects successful replies.");
            Logger.LogInformation($"Sending {RequestsPerCycle} requests every {PauseBetweenRequestsMs} millisecond.");
            return RunUntilCancelled(async () =>
            {
                var requests = GetRequests(RequestsPerCycle);
                foreach (var request in requests)
                {
                    await Bus.Send(request);
                } 
                await Tools.WaitUntilSuccess(() => AssertReceivedResponses(requests));  
                await Task.Delay(TimeSpan.FromMilliseconds(PauseBetweenRequestsMs));
            });
        }

        private Task SpamWithDummyRequests()
        {
            Logger.LogInformation("Starting alternative job - spamming with dummy requests.");
            return RunUntilCancelled(async () => 
            {
                var request = new DummyRequest();
                await Bus.Send(request);
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            });
        }

        private async Task RunUntilCancelled(Func<Task> func)
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                await func.Invoke();
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