using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.MessageHandler.Tests
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
                await Utilities.Tools.WaitUntilSuccess(async () =>
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
                
                var outboxCleanerHost = TestHostProvider.GetOutboxCleanerHost();
                var spammerHost = TestHostProvider.GetSpammerHost(outputQueue);
                
                var spammerHostTask = spammerHost.StartAsync(cancellationTokenSource.Token);
                var outboxCleanerHostTask = outboxCleanerHost.StartAsync(cancellationTokenSource.Token);
                
                var timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(OverrideConfig.DurationOfAcidTest);
                    cancellationTokenSource.Cancel();
                    await host.StopAsync(hardCancellationTokenSource.Token);
                    await outboxCleanerHost.StopAsync(hardCancellationTokenSource.Token);
                    await spammerHost.StopAsync(hardCancellationTokenSource.Token);
                }, hardCancellationTokenSource.Token);

                await host.StartAsync(cancellationTokenSource.Token);
                await spammerHostTask;
                await outboxCleanerHostTask;
                await timeoutTask;
            });
        }
    }
}