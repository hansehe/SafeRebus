using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.Extensions.Builder;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.NServiceBus.Host;
using SafeRebus.NServiceBus.Host.Contracts;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Integration.NServiceBus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class NServiceBusIntegrationTests
    {
        private static Dictionary<string, string> GetOverrideConfig()
        {
            var random = new Random();
            var number = random.Next();
            var randomInputQueue = $"NServiceBus.InputQueue_{number.ToString()}";
            var randomOutputQueue = $"NServiceBus.OutputQueue_{number.ToString()}";
            var overrideDict = new Dictionary<string, string>
            {
                {"rabbitMq:inputQueue", randomInputQueue},
                {"rabbitMq:outputQueue", randomOutputQueue},
            };
            
            return overrideDict;
        }
        
        private static IServiceProvider GetNServiceBusHostProvider()
        {
            var provider = new ServiceCollection()
                .ConfigureWithNServiceBusHost(GetOverrideConfig())
                .BuildServiceProvider();
            return provider;
        }

        [Fact]
        public async Task ReceiveNServiceBusMessage_Success()
        {
            var nServiceBusIntegrationAdditionalConfig = new Dictionary<string, string>();
            var nServiceBusSope = GetNServiceBusHostProvider().CreateScope();
            
            var inputQueue = nServiceBusSope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
            var outputQueue = nServiceBusSope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
            
            nServiceBusIntegrationAdditionalConfig["rabbitMq:inputQueue"] = outputQueue;
            nServiceBusIntegrationAdditionalConfig["rabbitMq:outputQueue"] = inputQueue;
            
            var rebusScope = TestServiceExecutor.GetServiceProvider(nServiceBusIntegrationAdditionalConfig).CreateScope();
            try
            {

                var endpointInstance = nServiceBusSope.ServiceProvider.GetService<IEndpointInstance>();
                var bus = rebusScope.ServiceProvider.GetService<IBus>();
                
                var repository = rebusScope.ServiceProvider.GetService<IResponseRepository>();
                var response = new NServiceBusResponse();

                await endpointInstance.Send(response);
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(response.Id)).Id.Should().Be(response.Id);
                }, TimeSpan.FromSeconds(3));
            }
            finally
            {
                nServiceBusSope.ServiceProvider.GetService<IRabbitMqUtility>().DeleteInputQueue();
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                rebusScope.ServiceProvider.GetService<IRabbitMqUtility>().DeleteInputQueue();
            }
        }
    }
}