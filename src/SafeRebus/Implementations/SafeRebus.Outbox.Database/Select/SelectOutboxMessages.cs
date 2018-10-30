using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Rebus.Serialization;
using SafeRebus.Config;
using SafeRebus.Outbox.Abstractions.Entities;

namespace SafeRebus.Outbox.Database.Select
{
    public static class SelectOutboxMessages
    {
        private const string SqlTemplate = "SELECT " +
                                           "(id, outgoing_message_headers, outgoing_message_object, outgoing_send_function)" +
                                           " FROM {0}.{1} WHERE timestamp <= NOW() - (INTERVAL '{2} seconds')";
        
        public static async Task<IEnumerable<OutboxMessage>> Select(
            IDbConnection dbConnection,
            IConfiguration configuration,
            TimeSpan threshold)
        {
            var objectSerializer = new ObjectSerializer();
            var seconds = threshold.TotalSeconds;
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.OutgoingMessagesTable,
                seconds.ToString(CultureInfo.InvariantCulture).Replace(",", "."));
            return (await dbConnection.QueryAsync<dynamic>(sql)).Select(
                    x =>
                    {
                        var outboxMessage = new OutboxMessage
                        {
                            Id = x.row[0],
                            Headers = objectSerializer.Deserialize(x.row[1]),
                            Message = objectSerializer.Deserialize(x.row[2]),
                            SendFunction = x.row[3]
                        };
                        return outboxMessage;
                    })
                .ToList();
        }
    }
}