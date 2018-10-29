using System;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Config;

namespace SafeRebus.TestUtilities
{  
    public class DatabaseFixture : IDisposable
    {
        public static string MigratedDatabaseSchema;

        private const string DropSchemaSqlTemplate = "DROP SCHEMA IF EXISTS {0} CASCADE;";
        
        public DatabaseFixture()
        {
            var provider = TestServiceExecutor.GetMigrationServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
                runner.MigrateUp();
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                MigratedDatabaseSchema = configuration.GetDbSchema();
            }
        }
        
        public void Dispose()
        {
            try
            {
                var provider = TestServiceExecutor.GetMigrationServiceProvider();
                using (var scope = provider.CreateScope())
                {
                    DeleteTestSchema(scope);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        private static void DeleteTestSchema(IServiceScope scope)
        {
            var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
            var sql = string.Format(DropSchemaSqlTemplate, MigratedDatabaseSchema);
            dbProvider.GetDbConnection().Execute(sql);
        }
    }
}