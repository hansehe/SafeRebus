using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;
using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.MessageHandler.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class ResponseRepositoryTests
    {
        [Fact]
        public async Task InsertAndCheckExists_Success()
        {
            var response = new SafeRebusResponse {Response = "some new random"};
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await repository.InsertResponse(response);
                dbProvider.GetDbTransaction().Commit();
            });
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var savedResponse = await repository.SelectResponse(response.Id);
                savedResponse.Response.Should().Be(response.Response);
                savedResponse.Id.Should().Be(response.Id);
            });
        }
        
        [Fact]
        public async Task MultipleInsertAndCheckExists_Success()
        {
            var allResponseIds = new List<Guid>();
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                for (var i = 0; i < 10; i++)
                {
                    var response = new SafeRebusResponse();
                    await repository.InsertResponse(response);
                    allResponseIds.Add(response.Id);
                }
                dbProvider.GetDbTransaction().Commit();
            });
            await TestServiceExecutor.ExecuteInScope(async scope =>
            {
                var repository = scope.ServiceProvider.GetService<IResponseRepository>();
                var savedResponseIds = (await repository.SelectResponses(allResponseIds)).Select(savedResponse => savedResponse.Id);
                allResponseIds.Select(responseId => savedResponseIds.Should().Contain(responseId));
            });
        }
    }
}