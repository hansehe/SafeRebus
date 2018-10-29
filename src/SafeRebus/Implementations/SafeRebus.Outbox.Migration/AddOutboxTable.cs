using FluentMigrator;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.Database;

namespace SafeRebus.Migration.Outbox
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
            Create.Table(Tables.OutboxTable).InSchema(schema)
                .WithColumn(Columns.Id).AsGuid().PrimaryKey()
                .WithColumn(Database.Outbox.Columns.Timestamp).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            var schema = Configuration.GetDbSchema();
            Delete.Table(Tables.OutboxTable).InSchema(schema);
        }
    }
}