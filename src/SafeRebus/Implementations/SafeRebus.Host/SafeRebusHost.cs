using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;
using SafeRebus.Config;
using SafeRebus.Contracts;
using SafeRebus.Contracts.Requests;

namespace SafeRebus.Host
{
    public class SafeRebusHost : IHostedService
    {
        private const long PauseBetweenRequestsMs = 500;
        private const int RequestsPerCycle = 10;
        private bool Cancel = false;
        private IList<Task> RunningTasks = new List<Task>();
        
        private readonly IBus Bus;

        public SafeRebusHost(
            IBus bus)
        {
            Bus = bus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var mainTask = Run();
            RunningTasks.Add(mainTask);
            if (BaseConfig.SendDummyRequests)
            {
                RunningTasks.Add(SpamWithDummyRequests());
            }
            return mainTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Cancel = true;
            foreach (var runningTask in RunningTasks)
            {
                await runningTask;
            }
        }

        private Task Run()
        {
            return RunUntilCancelled(async () => 
            {
                for (var i = 0; i < RequestsPerCycle; i++)
                {
                    await SendRequest();
                }
                await Task.Delay(TimeSpan.FromMilliseconds(PauseBetweenRequestsMs));   
            });
        }

        private Task SpamWithDummyRequests()
        {
            return RunUntilCancelled(async () => 
            {
                var request = new DummyRequest();
                await Bus.Send(request);    
            });
        }

        private async Task RunUntilCancelled(Func<Task> func)
        {
            while (!Cancel)
            {
                await func.Invoke();
            }
        }

        private async Task SendRequest()
        {
            var request = GetRequest();
            await Bus.Send(request);
        }

        private static SafeRebusRequest GetRequest()
        {
            return new SafeRebusRequest
            {
                RequestEnum = GetRandomRequestEnum()
            };
        }

        private static RequestEnum GetRandomRequestEnum()
        {
            var random = new Random();
            var enumValues = Enum.GetValues(typeof(RequestEnum));
            var randomEnum = (RequestEnum)enumValues.GetValue(random.Next(enumValues.Length));
            return randomEnum;
        }
    }
}