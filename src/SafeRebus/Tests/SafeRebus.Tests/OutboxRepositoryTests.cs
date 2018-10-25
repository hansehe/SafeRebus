using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class OutboxRepositoryTests
    {
        [Fact]
        public async Task InsertAndCheckExists_Success()
        {
            await TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var id = Guid.NewGuid();
                await outboxRepository.InsertMessageId(id);
                (await outboxRepository.MessageIdExists(id)).Should().BeTrue();
            });
        }
        
        [Fact]
        public async Task TryInsert_ShouldBeFalse()
        {
            await TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var id = Guid.NewGuid();
                (await outboxRepository.TryInsertMessageId(id)).Should().BeTrue();
                (await outboxRepository.TryInsertMessageId(id)).Should().BeFalse();
            });
        }
        
        [Fact]
        public async Task CleanOld_Success()
        {
            await TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var savedIds = new List<Guid>();
                for (int i = 0; i < 10; i++)
                {
                    var id = Guid.NewGuid();
                    await outboxRepository.InsertMessageId(id);
                    savedIds.Add(id);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
                await outboxRepository.CleanOldMessageIds(TimeSpan.Zero);
                foreach (var savedId in savedIds)
                {
                    (await outboxRepository.MessageIdExists(savedId)).Should().BeFalse();
                }
            });
        }
    }
}