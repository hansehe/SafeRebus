using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Responses;
using Xunit;
using FluentAssertions;
using SafeRebus.MessageHandler.MessageHandlers;
using SafeRebus.TestUtilities;

namespace SafeRebus.Tests
{
    public class RequestReplyTests
    {
        [Fact]
        public async Task AsyncRequestReply_Success()
        {
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var bus = scope.ServiceProvider.GetService<IBus>();
                var request = TestTools.GetRequest();
                await bus.Send(request);
                await TestTools.WaitUntilSuccess(async () =>
                    {
                        SafeRebusResponseMessageHandler.ReceivedResponses.Should().ContainKey(request.Id);
                    });
            });
        }
    }
}