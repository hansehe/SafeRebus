using System;
using System.Collections.Generic;
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
        public static string MigratedDatabaseSchema = "DefaultTestSchema";

        private const string DropSchemaSqlTemplate = "DROP SCHEMA IF EXISTS {0} CASCADE;";
        
        public DatabaseFixture()
        {
            var random = new Random();
            var randomSchema = $"SafeRebus_{random.Next().ToString()}";
            var additionalOverrideConfig = new Dictionary<string, string>
            {
                {"database:schema", randomSchema}
            };
            var provider = TestServiceProvider.GetMigrationServiceProvider(additionalOverrideConfig);
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
                var provider = TestServiceProvider.GetMigrationServiceProvider();
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