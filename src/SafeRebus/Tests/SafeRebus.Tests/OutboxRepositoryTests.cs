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
        public Task InsertAndCheckExists_Success()
        {
            return TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var id = Guid.NewGuid();
                await repository.InsertMessageId(id);
                (await repository.MessageIdExists(id)).Should().BeTrue();
            });
        }
        
        [Fact]
        public Task TryInsert_ShouldBeFalse()
        {
            return TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var id = Guid.NewGuid();
                (await repository.TryInsertMessageId(id)).Should().BeTrue();
                (await repository.TryInsertMessageId(id)).Should().BeFalse();
            });
        }
        
        [Fact]
        public Task CleanOld_Success()
        {
            return TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var savedIds = new List<Guid>();
                for (int i = 0; i < 10; i++)
                {
                    var id = Guid.NewGuid();
                    await repository.InsertMessageId(id);
                    savedIds.Add(id);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
                await repository.CleanOldMessageIds(TimeSpan.Zero);
                foreach (var savedId in savedIds)
                {
                    (await repository.MessageIdExists(savedId)).Should().BeFalse();
                }
            });
        }
    }
}