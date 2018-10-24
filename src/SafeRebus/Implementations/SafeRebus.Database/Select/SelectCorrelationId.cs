using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Database.Select
{
    public static class SelectCorrelationId
    {
        private const string SqlTemplate = "SELECT  {0}.{1} (@id)";
        
        public static Task Execute(
            IDbConnection dbConnection,
            IConfiguration configuration,
            Guid correlationId)
        {
            var @params = new DynamicParameters();
            @params.Add(Columns.Id, correlationId);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.OutboxTable);
            return dbConnection.ExecuteAsync(sql, @params);
        }
    }
}