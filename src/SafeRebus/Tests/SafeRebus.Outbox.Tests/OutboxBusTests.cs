using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Abstractions.Entities;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Outbox.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class OutboxBusTests
    {
        private static Dictionary<string, string> GetTransportMessageHeaders(string replyQueue)
        {
            return new Dictionary<string, string>
            {
                {Headers.ReturnAddress, replyQueue}
            };
        }
        
        private static TransportMessage GetTransportMessage(string replyQueue)
        {
            return new TransportMessage(GetTransportMessageHeaders(replyQueue), new byte[0]);
        }
        
        [Fact]
        public Task BeginTransactionAndCommit_Success()
        {
            return TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var originalQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
                var bus = scope.ServiceProvider.GetService<IOutboxBus>();
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var request = new SafeRebusRequest();
                await bus.BeginTransaction(GetTransportMessage(originalQueue));
                await bus.Send(request);
                await bus.Commit();
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(request.Id)).Id.Should().Be(request.Id);
                });
            });
        }
        
        [Fact]
        public async Task BeginTransactionAndNotCommit_ResendSuccess()
        {
            var request = new SafeRebusRequest();
            var additionalOverrideConfig = new Dictionary<string, string>();
            await  TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var inputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
                additionalOverrideConfig["rabbitMq:inputQueue"] = inputQueue;
                additionalOverrideConfig["rabbitMq:outputQueue"] = inputQueue;
                var bus = scope.ServiceProvider.GetService<IOutboxBus>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await bus.BeginTransaction(GetTransportMessage(inputQueue));
                await bus.Send(request);
                dbProvider.GetDbTransaction().Commit();
            });
            await Task.Delay(TimeSpan.FromSeconds(1));
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var outboxMessageCleaner = scope.ServiceProvider.GetService<IOutboxMessageCleaner>();
                await outboxMessageCleaner.CleanMessages(false);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }, additionalOverrideConfig);
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var outboxMessageRepository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(request.Id)).Id.Should().Be(request.Id);
                });
                var outboxMessages = await outboxMessageRepository.SelectOutboxMessagesBeforeThreshold(TimeSpan.Zero);
                outboxMessages.Should().BeEmpty();
            }, additionalOverrideConfig);
        }
        
        [Fact]
        public async Task BeginTransactionAndNotCommit_ResendWithReplySuccess()
        {
            var response = new SafeRebusResponse();
            var additionalOverrideConfig = new Dictionary<string, string>();
            await  TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var inputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
                var outputQueue = scope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
                additionalOverrideConfig["rabbitMq:inputQueue"] = inputQueue;
                additionalOverrideConfig["rabbitMq:outputQueue"] = outputQueue;
                var bus = scope.ServiceProvider.GetService<IOutboxBus>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await bus.BeginTransaction(GetTransportMessage(inputQueue));
                await bus.Reply(response);
                dbProvider.GetDbTransaction().Commit();
            }, additionalOverrideConfig);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var outboxMessageCleaner = scope.ServiceProvider.GetService<IOutboxMessageCleaner>();
                await outboxMessageCleaner.CleanMessages(false);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }, additionalOverrideConfig);
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var outboxMessageRepository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                await MessageHandler.Utilities.Tools.WaitUntilSuccess(async () =>
                {
                    (await repository.SelectResponse(response.Id)).Id.Should().Be(response.Id);
                });
                var outboxMessages = await outboxMessageRepository.SelectOutboxMessagesBeforeThreshold(TimeSpan.Zero);
                outboxMessages.Should().BeEmpty();
            }, additionalOverrideConfig);
        }
    }
}