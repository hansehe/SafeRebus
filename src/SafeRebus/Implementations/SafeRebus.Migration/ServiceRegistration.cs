using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SafeRebus.Abstractions;
using SafeRebus.Migration.MigrationModels;

namespace SafeRebus.Migration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddFluentMigratorCore()
                .AddScoped<IOptionsSnapshot<ProcessorOptions>>(sp =>
                {
                    var factory = sp.GetService<IOptionsFactory<ProcessorOptions>>();
                    var manager = new OptionsManager<ProcessorOptions>(factory);
                    manager.Value.ConnectionString = sp.GetService<IDbProvider>().GetConnectionString();
                    return manager;
                })
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .ScanIn(typeof(AddOutboxTable).Assembly).For.Migrations())
                .AddScoped<IVersionTableMetaData, VersionTable>()
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }
    }
}