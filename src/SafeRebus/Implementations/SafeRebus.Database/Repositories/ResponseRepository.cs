using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.Database.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IDbExecutor DbExecutor;

        public ResponseRepository(
            ILogger<ResponseRepository> logger,
            IConfiguration configuration,
            IDbExecutor dbExecutor)
        {
            Logger = logger;
            Configuration = configuration;
            DbExecutor = dbExecutor;
        }

        public Task InsertResponse(SafeRebusResponse response)
        {
            Logger.LogDebug($"Inserting response with id: {response.Id.ToString()}");
            return DbExecutor.ExecuteInTransactionAsync(dbConnection =>
                Insert.InsertResponse.Execute(dbConnection, Configuration, response));
        }

        public Task<SafeRebusResponse> SelectResponse(Guid id)
        {
            Logger.LogDebug($"Selecting response with id: {id.ToString()}");
            return DbExecutor.SelectInTransactionAsync(dbConnection =>
                Select.SelectResponse.Select(dbConnection, Configuration, id));
        }

        public Task<IEnumerable<SafeRebusResponse>> SelectResponses(IEnumerable<Guid> ids)
        {
            Logger.LogDebug($"Selecting multiple responses");
            return DbExecutor.SelectInTransactionAsync(dbConnection =>
                Select.SelectResponses.Select(dbConnection, Configuration, ids.ToArray()));
        }
    }
}