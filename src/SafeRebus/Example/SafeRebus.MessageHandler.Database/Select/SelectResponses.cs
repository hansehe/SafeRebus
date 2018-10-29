using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.MessageHandler.Database.Select
{
    public static class SelectResponses
    {
        private const string SqlTemplate = "SELECT * FROM {0}.{1} WHERE id IN ({2})";
        
        public static Task<IEnumerable<SafeRebusResponse>> Select(
            IDbConnection dbConnection,
            IConfiguration configuration,
            Guid[] ids)
        {
            var i = 1;
            var @params = new DynamicParameters();
            var parameters = string.Join(", ", ids.Select(x => $"@id{i++}"));
            i = 1;
            foreach (var id in ids)
            {
                @params.Add($"@id{i++}", id);
            }
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                SafeRebus.MessageHandler.Database.Tables.ResponseTable,
                parameters);
            return dbConnection.QueryAsync<SafeRebusResponse>(sql, @params);
        }
    }
}