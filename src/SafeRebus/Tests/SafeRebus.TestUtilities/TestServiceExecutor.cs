using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Builder;

namespace SafeRebus.TestUtilities
{
    public static class TestServiceExecutor
    {
        public static async Task ExecuteInScope(Func<IServiceScope, Task> action)
        {
            var provider = GetServiceProvider();
            using (var scope = provider.CreateScope())
            {
                await action.Invoke(scope);
            }
        }
        
        public static async Task ExecuteInDbTransactionScopeWithRollback(Func<IServiceScope, Task> action)
        {
            await ExecuteInScope(async scope =>
            {
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                using (var transaction = dbProvider.GetDbTransaction())
                {
                    try
                    {
                        await action.Invoke(scope);
                    }
                    finally
                    {
                        transaction.Rollback();
                    }
                }
            });
        }
        
        public static IServiceProvider GetServiceProvider()
        {
            var overrideConfig = GetOverrideConfig();
            overrideConfig["database:schema"] = DatabaseFixture.MigratedDatabaseSchema;
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebus(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetMigrationServiceProvider()
        {
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMigration(GetOverrideConfig())
                .BuildServiceProvider();
            return provider;
        }

        private static Dictionary<string, string> GetOverrideConfig()
        {
            var random = new Random();
            var randomQueue = $"SafeRebus.InputQueue_{random.Next().ToString()}";
            var randomSchema = $"SafeRebus_{random.Next().ToString()}";
            var overrideDict = new Dictionary<string, string>
            {
                {"rabbitMq:inputQueue", randomQueue},
                {"rabbitMq:outputQueue", randomQueue},
                {"database:schema", randomSchema}
            };
            return overrideDict;
        }
    }
}