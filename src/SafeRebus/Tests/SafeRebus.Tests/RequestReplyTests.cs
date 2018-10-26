using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.Contracts.Responses;
using SafeRebus.TestUtilities;
using SafeRebus.Utilities;

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
                var request = new SafeRebusResponse();
                await bus.Send(request);
                await Tools.WaitUntilSuccess(async () =>
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
                var spammerTask = TestServiceExecutor.StartSpammerHost(cancellationTokenSource.Token, outputQueue);
                
                var timeoutTask = Task.Run(async () =>
                {
                    var timeout = TimeSpan.FromSeconds(DurationOfAcidTestSec);
                    await Task.Delay(timeout);
                    cancellationTokenSource.Cancel();
                    await host.StopAsync(hardCancellationTokenSource.Token);
                });
                
                await host.StartAsync(cancellationTokenSource.Token);
                await timeoutTask;
                await spammerTask;
            });
        }
    }
}