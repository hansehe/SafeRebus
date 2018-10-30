using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Outbox.Database.Delete
{
    public static class DeleteOutboxMessages
    {
        private const string SqlTemplate = "DELETE FROM {0}.{1} WHERE id IN ({2})";
        
        public static Task Delete(
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
                Tables.OutgoingMessagesTable,
                parameters);
            return dbConnection.ExecuteAsync(sql, @params);
        }
    }
}