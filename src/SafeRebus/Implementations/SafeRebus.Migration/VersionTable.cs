using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.Migration
{
    public class VersionTable : DefaultVersionTableMetaData
    {
        private readonly IConfiguration Configuration;

        public VersionTable(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override string SchemaName => Configuration.GetDbSchema();
    }
}