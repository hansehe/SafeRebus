using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Config;
using SafeRebus.MessageHandler.Host;
using SafeRebus.MessageHandler.Utilities;

namespace SafeRebus.NServiceBus.Host
{
    public class NServiceBusHost : IHostedService
    {
        private Task MainTask;
        
        private int ResponsesPerCycle => Configuration.GetHostRequestsPerCycle();
        private string OutputQueue => Configuration.GetRabbitMqOutputQueue();
        
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IEndpointInstance EndpointInstance;
        private readonly IResponseRepository ResponseRepository;

        public NServiceBusHost(
            ILogger<NServiceBusHost> logger,
            IConfiguration configuration,
            IEndpointInstance endpointInstance,
            IResponseRepository responseRepository)
        {
            Logger = logger;
            Configuration = configuration;
            EndpointInstance = endpointInstance;
            ResponseRepository = responseRepository;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting NServiceBus host");
            MainTask = Task.Run(() => Run(cancellationToken));
            return MainTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopping NServiceBus host");
            MainTask.Wait(cancellationToken);
            return Task.CompletedTask;
        }

        private Task Run(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting nServiceBus host job - sending responses, and expects to find them in the database.");
            Logger.LogInformation($"Sending {ResponsesPerCycle} responses during each cycle.");
            return SafeRebusHostHelper.RunUntilCancelled(async () =>
            {
                var responses = SafeRebusHostHelper.GetResponses(ResponsesPerCycle);
                foreach (var response in responses)
                {
                    await EndpointInstance.Send(OutputQueue, response);
                } 
                await Tools.WaitUntilSuccess(
                    () => ResponseRepository.AssertReceivedResponses(responses));
                Logger.LogInformation("Successfully received all responses!");
            }, cancellationToken);
        }
    }
}