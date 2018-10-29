using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.MessageHandler.Database.Insert
{
    public static class InsertResponse
    {
        private const string SqlTemplate = "INSERT INTO {0}.{1} (id, response) VALUES (@id, @response)";
        
        public static Task Execute(
            IDbConnection dbConnection,
            IConfiguration configuration,
            SafeRebusResponse response)
        {
            var @params = new DynamicParameters();
            @params.Add(SafeRebus.Database.Columns.Id, response.Id);
            @params.Add(SafeRebus.MessageHandler.Database.Columns.Response, response.Response);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                SafeRebus.MessageHandler.Database.Tables.ResponseTable);
            return dbConnection.ExecuteAsync(sql, @params);
        }
    }
}