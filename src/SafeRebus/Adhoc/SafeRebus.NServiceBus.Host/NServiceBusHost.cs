using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Transport;
using SafeRebus.Config;
using SafeRebus.NServiceBus.Host.Contracts;

namespace SafeRebus.NServiceBus.Host
{
    public class NServiceBusHost : IHostedService
    {
        private Task MainTask;
        
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;

        public NServiceBusHost(
            ILogger<NServiceBusHost> logger,
            IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
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
            return Task.CompletedTask;
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            var endpointConfiguration = new EndpointConfiguration(Configuration.GetRabbitMqInputQueue());
            
            endpointConfiguration.UseTransport<RabbitMQTransport>()
                .ConnectionString(Configuration.GetNServiceBusConnectionString())
                .UseConventionalRoutingTopology()
                .UseDurableExchangesAndQueues(false)
                .Routing()
                .RouteToEndpoint(typeof(NServiceBusDummyRequest), Configuration.GetRabbitMqOutputQueue());

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            while (!cancellationToken.IsCancellationRequested)
            {
            }

            await endpointInstance.Stop();
        }
    }
}