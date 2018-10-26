using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Database.Delete
{
    public static class DeleteCorrelationIdsFromTimestamp
    {
        private const string SqlTemplate = "DELETE FROM {0}.{1} WHERE timestamp < NOW() - (INTERVAL '{2} seconds')";
        
        public static Task Delete(
            IDbConnection dbConnection,
            IConfiguration configuration,
            TimeSpan tooOldThreshold)
        {
            var secondsThreshold = tooOldThreshold.TotalSeconds;
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.OutboxTable,
                secondsThreshold.ToString(CultureInfo.InvariantCulture).Replace(",", "."));
           return dbConnection.QueryAsync(sql);
        }
    }
}