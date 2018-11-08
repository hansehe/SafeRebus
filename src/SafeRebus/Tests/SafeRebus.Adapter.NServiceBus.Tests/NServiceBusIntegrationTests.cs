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
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Adapter.NServiceBus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class NServiceBusIntegrationTests
    {
        [Fact]
        public Task SendFromNServiceBusToRebus_Success()
        {
            return NServiceBusTestUtilities.ExecuteInNServiceBusScope(async (scope, rebusBus) =>
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
        public Task SendFromNServiceBusToRebusWithReplyFromRebus_Success()
        {
            return NServiceBusTestUtilities.ExecuteInNServiceBusScope(async (scope, rebusBus) =>
            {
                var outputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
                var endpointInstance = scope.ServiceProvider.GetService<IEndpointInstance>();
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var request = new SafeRebusRequest();
                var request2 = new SafeRebusRequest();

                await endpointInstance.Send(outputQueue, request);
                await endpointInstance.Send(outputQueue, request2);
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(request.Id)).Id.Should().Be(request.Id);
                    (await repository.SelectResponse(request2.Id)).Id.Should().Be(request2.Id);
                }, TimeSpan.FromSeconds(3)); 
            });
        }
    }
}