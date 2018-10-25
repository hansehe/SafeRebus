using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Xunit;
using FluentAssertions;
using SafeRebus.MessageHandler;
using SafeRebus.TestUtilities;

namespace SafeRebus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class RequestReplyTests
    {
        [Fact]
        public async Task AsyncRequestReply_Success()
        {
            var serviceProvider = TestServiceExecutor.GetServiceProvider();
            var bus = serviceProvider.GetRequiredService<IBus>();
            var request = TestTools.GetRequest();
            await bus.Send(request);
            await TestTools.WaitUntilSuccess(async () =>
                {
                    SafeRebusResponseMessageHandler.ReceivedResponses.Should().ContainKey(request.Id);
                });
        }
    }
}