using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class ResponseRepositoryTests
    {
        [Fact]
        public Task InsertAndCheckExists_Success()
        {
            return TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var response = new SafeRebusResponse();
                await repository.InsertResponse(response);
                (await repository.SelectResponse(response.Id)).Id.Should().Be(response.Id);
            });
        }
        
        [Fact]
        public Task MultipleInsertAndCheckExists_Success()
        {
            return TestServiceExecutor.ExecuteInDbTransactionScopeWithRollback(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var allResponseIds = new List<Guid>();
                for (var i = 0; i < 10; i++)
                {
                    var response = new SafeRebusResponse();
                    await repository.InsertResponse(response);
                    allResponseIds.Add(response.Id);
                }
                (await repository.SelectResponses(allResponseIds)).Count().Should().Be(allResponseIds.Count);
            });
        }
    }
}