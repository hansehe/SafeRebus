using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Abstractions.Entities;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class OutboxMessageRepositoryTests
    {
        private static OutboxMessage GetOutboxMessage()
        {
            return new OutboxMessage
            {
                Headers = new Dictionary<string, string>
                {
                    {
                        "message_id", "123456"
                    }
                },
                Message = new SafeRebusResponse(),
                SendFunction = "someSendFunction"
            };
        }
        
        [Fact]
        public async Task InsertSelectAndDelete_Success()
        {
            var outboxMessage = GetOutboxMessage();
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await repository.InsertOutboxMessage(outboxMessage);
                dbProvider.GetDbTransaction().Commit();
            });
            await Task.Delay(TimeSpan.FromMilliseconds(700));
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await repository.InsertOutboxMessage(GetOutboxMessage());
                dbProvider.GetDbTransaction().Commit();
            });
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                var insertedOutboxMessages = (await repository.SelectOutboxMessagesBeforeThreshold(TimeSpan.FromSeconds(1))).ToArray();
                
                insertedOutboxMessages.Count().Should().Be(1);
                var insertedOutboxMessage = insertedOutboxMessages.First();

                insertedOutboxMessage.Id.Should().Be(outboxMessage.Id);
                insertedOutboxMessage.Headers.Should().BeEquivalentTo(outboxMessage.Headers);
                insertedOutboxMessage.Message.Should().BeEquivalentTo(outboxMessage.Message);
                insertedOutboxMessage.SendFunction.Should().Be(outboxMessage.SendFunction);
            });
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                await repository.DeleteOutboxMessage(outboxMessage.Id);
            });
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxMessageRepository>();
                var insertedOutboxMessages = (await repository.SelectOutboxMessagesBeforeThreshold(TimeSpan.FromSeconds(1))).ToArray();
                insertedOutboxMessages.Should().BeEmpty();
            });
        }
    }
}