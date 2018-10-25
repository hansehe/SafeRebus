using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Responses;
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
    }
}