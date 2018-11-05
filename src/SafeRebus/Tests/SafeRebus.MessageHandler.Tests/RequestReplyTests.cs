using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.TestUtilities;

namespace SafeRebus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class RequestReplyTests
    {
        [Fact]
        public Task AsyncRequestReply_Success()
        {
            return TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var bus = scope.ServiceProvider.GetService<IBus>();
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var request = new SafeRebusRequest();
                await bus.Send(request);
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(request.Id)).Id.Should().Be(request.Id);
                });
            });
        }
        
        [Fact]
        public Task MultipleAsyncRequestRepliesWithJokerExceptions_AcidTest_Success()
        {
            return TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var host = scope.ServiceProvider.GetService<IHostedService>();
                var cancellationTokenSource = new CancellationTokenSource();
                var hardCancellationTokenSource = new CancellationTokenSource();
                
                var inputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
                var outputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
                var schema = scope.ServiceProvider.GetService<IConfiguration>().GetDbSchema();
                var outboxCleanerHost = TestServiceExecutor.StartOutboxCleanerHost(cancellationTokenSource.Token, schema);
                var spammerHost = TestServiceExecutor.StartSpammerHost(cancellationTokenSource.Token, outputQueue);
                var nServiceBusHost = TestServiceExecutor.GetNServiceBusHost(inputQueue);
                
                var timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(OverrideConfig.DurationOfAcidTest);
                    cancellationTokenSource.Cancel();
                    await host.StopAsync(hardCancellationTokenSource.Token);
                    await nServiceBusHost.StopAsync(hardCancellationTokenSource.Token);
                    await outboxCleanerHost.StopAsync(hardCancellationTokenSource.Token);
                    await spammerHost.StopAsync(hardCancellationTokenSource.Token);
                }, hardCancellationTokenSource.Token);

                var nServiceBusHostTask = nServiceBusHost.StartAsync(cancellationTokenSource.Token);
                await host.StartAsync(cancellationTokenSource.Token);
                await nServiceBusHostTask;
                await timeoutTask;
            });
        }
    }
}