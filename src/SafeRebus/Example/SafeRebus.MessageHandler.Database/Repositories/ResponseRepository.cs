using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.MessageHandler.Database.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration Configuration;
        private readonly IDbProvider DbProvider;

        public ResponseRepository(
            ILogger<ResponseRepository> logger,
            IConfiguration configuration,
            IDbProvider dbProvider)
        {
            Logger = logger;
            Configuration = configuration;
            DbProvider = dbProvider;
        }

        public Task InsertResponse(SafeRebusResponse response)
        {
            Logger.LogDebug($"Inserting response with id: {response.Id.ToString()}");
            return Insert.InsertResponse.Execute(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                response);
        }

        public Task<SafeRebusResponse> SelectResponse(Guid id)
        {
            Logger.LogDebug($"Selecting response with id: {id.ToString()}");
            return Select.SelectResponse.Select(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                id);
        }

        public Task<IEnumerable<SafeRebusResponse>> SelectResponses(IEnumerable<Guid> ids)
        {
            Logger.LogDebug($"Selecting multiple responses");
            return Select.SelectResponses.Select(
                DbProvider.GetDbTransaction().Connection,
                Configuration,
                ids.ToArray());
        }
    }
}