using FluentMigrator;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;
using SafeRebus.Database;

namespace SafeRebus.Migration.MigrationModels
{
    [Migration(1)]
    public class AddResponseTable : FluentMigrator.Migration
    {
        private readonly IConfiguration Configuration;

        public AddResponseTable(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public override void Up()
        {
            var schema = Configuration.GetDbSchema();
            Create.Table(Tables.ResponseTable).InSchema(schema)
                .WithColumn(Columns.Id).AsGuid().PrimaryKey()
                .WithColumn(Columns.Response).AsString();
        }

        public override void Down()
        {
            var schema = Configuration.GetDbSchema();
            Delete.Table(Tables.ResponseTable).InSchema(schema);
        }
    }
}