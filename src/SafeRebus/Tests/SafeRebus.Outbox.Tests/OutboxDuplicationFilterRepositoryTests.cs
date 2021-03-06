using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Outbox.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class OutboxDuplicationFilterRepositoryTests
    {
        [Fact]
        public Task InsertAndCheckExists_Success()
        {
            return TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxDuplicationFilterRepository>();
                var id = Guid.NewGuid();
                await repository.InsertMessageId(id);
                (await repository.MessageIdExists(id)).Should().BeTrue();
            });
        }
        
        [Fact]
        public Task TryInsert_ShouldBeFalse()
        {
            return TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxDuplicationFilterRepository>();
                var id = Guid.NewGuid();
                (await repository.TryInsertMessageId(id)).Should().BeTrue();
                (await repository.TryInsertMessageId(id)).Should().BeFalse();
            });
        }
        
        [Fact]
        public async Task CleanOld_Success()
        {
            var savedIds = new List<Guid>();
            var saveIdsShouldNotBeDeleted = new List<Guid>();
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxDuplicationFilterRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                for (var i = 0; i < 10; i++)
                {
                    var id = Guid.NewGuid();
                    await repository.InsertMessageId(id);
                    savedIds.Add(id);
                }
                dbProvider.GetDbTransaction().Commit();
            });
            await Task.Delay(TimeSpan.FromSeconds(2));
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxDuplicationFilterRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                for (var i = 0; i < 10; i++)
                {
                    var id = Guid.NewGuid();
                    await repository.InsertMessageId(id);
                    saveIdsShouldNotBeDeleted.Add(id);
                }
                dbProvider.GetDbTransaction().Commit();
            });
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IOutboxDuplicationFilterRepository>();
                await repository.CleanOldMessageIds(TimeSpan.FromSeconds(1.5));
                foreach (var savedId in savedIds)
                {
                    (await repository.MessageIdExists(savedId)).Should().BeFalse();
                }
                foreach (var savedIdShouldExist in saveIdsShouldNotBeDeleted)
                {
                    (await repository.MessageIdExists(savedIdShouldExist)).Should().BeTrue();
                }
            });
        }
    }
}