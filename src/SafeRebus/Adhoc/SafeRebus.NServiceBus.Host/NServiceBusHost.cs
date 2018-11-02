using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace SafeRebus.NServiceBus.Host
{
    public class NServiceBusHost : IHostedService
    {
        private Task MainTask;
        
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IEndpointInstance EndpointInstance;

        public NServiceBusHost(
            ILogger<NServiceBusHost> logger,
            IConfiguration configuration,
            IEndpointInstance endpointInstance)
        {
            Logger = logger;
            Configuration = configuration;
            EndpointInstance = endpointInstance;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting NServiceBus host");
            MainTask = Task.Run(async () => await Run(cancellationToken));
            return MainTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping NServiceBus host");
            MainTask.Wait(cancellationToken);
            return EndpointInstance.Stop();
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
            }
        }
    }
}