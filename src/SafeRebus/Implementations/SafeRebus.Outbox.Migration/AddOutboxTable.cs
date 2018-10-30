using FluentMigrator;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.Database;
using SafeRebus.Outbox.Database;

namespace SafeRebus.Outbox.Migration
{
    [Migration(0)]
    public class AddOutboxTable : FluentMigrator.Migration
    {
        private readonly IConfiguration Configuration;

        public AddOutboxTable(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public override void Up()
        {
            var schema = Configuration.GetDbSchema();
            
            Create.Table(Tables.DuplicationFilterTable).InSchema(schema)
                .WithColumn(CommonColumns.Id).AsGuid().PrimaryKey()
                .WithColumn(Columns.Timestamp).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime).Indexed();

            Create.Table(Tables.OutgoingMessagesTable).InSchema(schema)
                .WithColumn(CommonColumns.Id).AsGuid().PrimaryKey()
                .WithColumn(Columns.OutgoingMessageObject).AsBinary()
                .WithColumn(Columns.OutgoingMessageHeaders).AsBinary()
                .WithColumn(Columns.OutgoingSendFunction).AsString()
                .WithColumn(Columns.Timestamp).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime).Indexed();
        }

        public override void Down()
        {
            var schema = Configuration.GetDbSchema();
            Delete.Table(Tables.DuplicationFilterTable).InSchema(schema);
        }
    }
}