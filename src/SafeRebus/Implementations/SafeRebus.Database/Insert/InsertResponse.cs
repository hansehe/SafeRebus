using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.Database.Insert
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
            @params.Add(Columns.Id, response.Id);
            @params.Add(Columns.Response, response.Response);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.ResponseTable);
            return dbConnection.ExecuteAsync(sql, @params);
        }
    }
}