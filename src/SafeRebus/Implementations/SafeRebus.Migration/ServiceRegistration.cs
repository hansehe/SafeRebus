using System.Collections.Generic;
using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SafeRebus.Abstractions;

namespace SafeRebus.Migration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection, IEnumerable<Assembly> assembliesWithMigrationModels)
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
                    .ScanIn().AssembliesForMigrations(assembliesWithMigrationModels))
                .AddScoped<IVersionTableMetaData, VersionTable>()
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }

        private static IScanInBuilder AssembliesForMigrations(this IScanInBuilder scanInBuilder, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            foreach (var assemblyWithMigrationModels in assembliesWithMigrationModels)
            {
                scanInBuilder.ScanIn(assemblyWithMigrationModels).For.Migrations();
            }
            return scanInBuilder;
        }
    }
}