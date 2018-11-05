using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Adapter.NServiceBus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class NServiceBusIntegrationTests
    {
        [Fact]
        public Task ReceiveNServiceBusMessage_Success()
        {
            return NServiceBusTestUtilities.ExecuteInNServiceBusScope(async scope =>
            {
                var outputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
                var endpointInstance = scope.ServiceProvider.GetService<IEndpointInstance>();
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var response = new SafeRebusResponse();

                await endpointInstance.Send(outputQueue, response);
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(response.Id)).Id.Should().Be(response.Id);
                }, TimeSpan.FromSeconds(3)); 
            });
        }
        
        [Fact]
        public Task ReceiveNServiceBusMessagesFromNServiceBusHost_Success()
        {
            return NServiceBusTestUtilities.ExecuteInNServiceBusScope(async scope =>
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var hardCancellationTokenSource = new CancellationTokenSource();
                
                var nServiceBusHost = scope.ServiceProvider.GetService<IHostedService>();

                var timeoutTask = Task.Run(async() =>
                {
                    await Task.Delay(OverrideConfig.DurationOfAcidTest);
                    cancellationTokenSource.Cancel();
                    await nServiceBusHost.StopAsync(hardCancellationTokenSource.Token);
                }, hardCancellationTokenSource.Token);
                
                await nServiceBusHost.StartAsync(cancellationTokenSource.Token);
                await timeoutTask;
            });
        }
    }
}