using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public static async Task StartSpammerHost(CancellationToken cancellationToken, string outputQueue)
        {
            var provider = GetSpammerServiceProvider(outputQueue);
            using (var scope = provider.CreateScope())
            {
                var spammerHost = scope.ServiceProvider.GetService<IHostedService>();
                await spammerHost.StartAsync(cancellationToken);
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
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            overrideConfig["database:schema"] = DatabaseFixture.MigratedDatabaseSchema;
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebus(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetSpammerServiceProvider(string outputQueue)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            overrideConfig["rabbitMq:outputQueue"] = outputQueue;
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusSpammer(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetMigrationServiceProvider()
        {
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMigration(OverrideConfig.GetOverrideConfig())
                .BuildServiceProvider();
            return provider;
        }
    }
}