using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.NServiceBus.Host;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Integration.NServiceBus.Tests
{
    public class NServiceBusIntegrationTests
    {
        private static IServiceProvider GetNServiceBusHostProvider()
        {
            var provider = new ServiceCollection()
                .ConfigureWithNServiceBusHost()
                .BuildServiceProvider();
            return provider;
        }

        [Fact]
        public async Task ReceiveNServiceBusMessage_Success()
        {
            var nServiceBusIntegrationAdditionalConfig = new Dictionary<string, string>();
            var cancellationTokenSource = new CancellationTokenSource();
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var inputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
                nServiceBusIntegrationAdditionalConfig["rabbitMq:outputQueue"] = inputQueue;
                var host = scope.ServiceProvider.GetService<IHostedService>();
                await host.StartAsync(cancellationTokenSource.Token);
            }, provider: GetNServiceBusHostProvider());
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var bus = scope.ServiceProvider.GetService<IBus>();
                var request = new DummyRequest();
                await bus.Send(request);
                await Task.Delay(TimeSpan.FromSeconds(1));
                cancellationTokenSource.Cancel();
            }, nServiceBusIntegrationAdditionalConfig);

//            await nServiceBusHostTask;
        }
    }
}