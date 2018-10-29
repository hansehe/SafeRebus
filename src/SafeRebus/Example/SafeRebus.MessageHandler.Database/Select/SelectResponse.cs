using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.MessageHandler.Database.Select
{
    public static class SelectResponse
    {
        private const string SqlTemplate = "SELECT * FROM {0}.{1} WHERE id = @id";
        
        public static Task<SafeRebusResponse> Select(
            IDbConnection dbConnection,
            IConfiguration configuration,
            Guid id)
        {
            var @params = new DynamicParameters();
            @params.Add(SafeRebus.Database.Columns.Id, id);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                SafeRebus.MessageHandler.Database.Tables.ResponseTable);
            return dbConnection.QuerySingleAsync<SafeRebusResponse>(sql, @params);
        }
    }
}