using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Database.Delete
{
    public static class DeleteCorrelationIdsFromTimestamp
    {
        private const string SqlTemplate = "DELETE FROM {0}.{1} WHERE timestamp < @timestamp";
        
        public static Task Delete(
            IDbConnection dbConnection,
            IConfiguration configuration,
            DateTime tooOldThreshold)
        {
            var @params = new DynamicParameters();
            @params.Add(Columns.Timestamp, tooOldThreshold);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.OutboxTable);
            return dbConnection.ExecuteAsync(sql, @params);
        }
    }
}