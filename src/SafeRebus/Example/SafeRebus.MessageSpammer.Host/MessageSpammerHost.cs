using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using SafeRebus.MessageHandler.Contracts.Requests;

namespace SafeRebus.MessageSpammer.Host
{
    public class MessageSpammerHost : IHostedService
    {
        private readonly ILogger Logger;
        private readonly IBus Bus;
        private Task MainTask;

        public MessageSpammerHost(
            ILogger<MessageSpammerHost> logger,
            IBus bus)
        {
            Logger = logger;
            Bus = bus;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting message spammer host.");
            MainTask = Task.Run(() => Run(cancellationToken));
            return MainTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping message spammer host.");
            MainTask.Wait(cancellationToken);
            return Task.CompletedTask;
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Bus.Send(new DummyRequest());
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }
    }
}