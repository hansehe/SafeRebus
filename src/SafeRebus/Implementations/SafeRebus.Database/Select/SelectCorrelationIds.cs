using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Database.Select
{
    public static class SelectCorrelationIds
    {
        private const string SqlTemplate = "SELECT id FROM {0}.{1} WHERE id = @id";
        
        public static Task<IEnumerable<Guid>> Select(
            IDbConnection dbConnection,
            IConfiguration configuration,
            Guid correlationId)
        {
            var @params = new DynamicParameters();
            @params.Add(Columns.Id, correlationId);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.OutboxTable);
            return dbConnection.QueryAsync<Guid>(sql, @params);
        }
    }
}