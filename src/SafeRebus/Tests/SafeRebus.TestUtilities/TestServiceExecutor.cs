using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SafeRebus.Abstractions;
using SafeRebus.MessageHandler.Builder;
using SafeRebus.NServiceBus.Host;

namespace SafeRebus.TestUtilities
{
    public static class TestServiceExecutor
    {
        public static async Task ExecuteInScope(Func<IServiceScope, Task> action, 
            Dictionary<string, string> additionalOverrideConfig = null,
            IServiceProvider provider = null)
        {
            provider = provider ?? GetServiceProvider(additionalOverrideConfig);
            using (var scope = provider.CreateScope())
            {
                try
                {
                    await action.Invoke(scope);
                }
                finally
                {
                    TryDeleteTestQueue(scope);
                }
            }
        }
        
        public static IHostedService StartOutboxCleanerHost(CancellationToken cancellationToken, string schema)
        {
            var provider = GetOutboxCleanerServiceProvider(schema);
            var scope = provider.CreateScope();
            var outboxCleanerHost = scope.ServiceProvider.GetService<IHostedService>();
            outboxCleanerHost.StartAsync(cancellationToken);
            return outboxCleanerHost;
        }

        public static IHostedService StartSpammerHost(CancellationToken cancellationToken, string outputQueue)
        {
            var provider = GetSpammerServiceProvider(outputQueue);
            var scope = provider.CreateScope();
            var spammerHost = scope.ServiceProvider.GetService<IHostedService>();
            spammerHost.StartAsync(cancellationToken);
            return spammerHost;
        }
        
        public static IHostedService GetNServiceBusHost(string safeRebusInputQueue)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            overrideConfig["rabbitMq:outputQueue"] = safeRebusInputQueue;
            var provider = GetNServiceBusServiceProvider(overrideConfig);
            var scope = provider.CreateScope();
            var nServiceBusHost = scope.ServiceProvider.GetService<IHostedService>();
            return nServiceBusHost;
        }
        
        
        public static IServiceProvider GetServiceProvider(Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            overrideConfig["database:schema"] = DatabaseFixture.MigratedDatabaseSchema;
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMessageHandler(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetOutboxCleanerServiceProvider(string schema)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            overrideConfig["database:schema"] = schema;
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusOutboxCleanerHost(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetSpammerServiceProvider(string outputQueue)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            overrideConfig["rabbitMq:inputQueue"] = outputQueue;
            overrideConfig["rabbitMq:outputQueue"] = outputQueue;
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMessageSpammer(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetNServiceBusServiceProvider(Dictionary<string, string> overrideConfig = null)
        {
            overrideConfig = overrideConfig ?? OverrideConfig.GetOverrideConfig();
            overrideConfig["database:schema"] = DatabaseFixture.MigratedDatabaseSchema;
            var provider = new ServiceCollection()
                .ConfigureWithNServiceBusHost(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetMigrationServiceProvider()
        {
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMessageHandlerMigration(OverrideConfig.GetOverrideConfig())
                .BuildServiceProvider();
            return provider;
        }

        private static void TryDeleteTestQueue(IServiceScope scope)
        {
            try
            {
                var rabbitMqUtility = scope.ServiceProvider.GetService<IRabbitMqUtility>();
                rabbitMqUtility.DeleteInputQueue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}