using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Rebus.Serialization;
using SafeRebus.Config;
using SafeRebus.Database;
using SafeRebus.Outbox.Abstractions.Entities;

namespace SafeRebus.Outbox.Database.Insert
{
    public static class InsertOutboxMessage
    {
        private const string SqlTemplate = "INSERT INTO {0}.{1} " +
                                           "(id, outgoing_message_headers, outgoing_message_object, outgoing_send_function)" +
                                           " VALUES " +
                                           "(@id, @outgoing_message_headers, @outgoing_message_object, @outgoing_send_function)";
        
        public static Task Execute(
            IDbConnection dbConnection,
            IConfiguration configuration,
            OutboxMessage outboxMessage)
        {
            var objectSerializer = new ObjectSerializer();
            var @params = new DynamicParameters();
            @params.Add(CommonColumns.Id, outboxMessage.Id);
            @params.Add(Columns.OutgoingMessageHeaders, objectSerializer.Serialize(outboxMessage.Headers));
            @params.Add(Columns.OutgoingMessageObject, objectSerializer.Serialize(outboxMessage.Message));
            @params.Add(Columns.OutgoingSendFunction, outboxMessage.SendFunction);
            var sql = string.Format(SqlTemplate,
                configuration.GetDbSchema(),
                Tables.OutgoingMessagesTable);
            return dbConnection.ExecuteAsync(sql, @params);
        }
    }
}