using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Database.Insert
{
    public static class InsertCorrelationId
    {
        private const string SqlTemplate = "INSERT INTO {0}.{1} (id) VALUES (@id)";
        
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