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
        private const long DurationOfAcidTestSec = 10;
        
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
                
                var outputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
                var schema = scope.ServiceProvider.GetService<IConfiguration>().GetDbSchema();
                var outboxCleanerHost = TestServiceExecutor.StartOutboxCleanerHost(cancellationTokenSource.Token, schema);
                var spammerHost = TestServiceExecutor.StartSpammerHost(cancellationTokenSource.Token, outputQueue);
                
                var timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(DurationOfAcidTestSec));
                    cancellationTokenSource.Cancel();
                    await host.StopAsync(hardCancellationTokenSource.Token);
                    await outboxCleanerHost.StopAsync(hardCancellationTokenSource.Token);
                    await spammerHost.StopAsync(hardCancellationTokenSource.Token);
                }, hardCancellationTokenSource.Token);
                
                await host.StartAsync(cancellationTokenSource.Token);
                await timeoutTask;
            });
        }
    }
}