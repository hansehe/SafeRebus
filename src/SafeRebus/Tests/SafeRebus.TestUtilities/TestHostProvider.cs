using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SafeRebus.TestUtilities
{
    public static class TestHostProvider
    {
        public static IHostedService GetOutboxCleanerHost()
        {
            var provider = TestServiceProvider.GetOutboxCleanerServiceProvider();
            var scope = provider.CreateScope();
            var outboxCleanerHost = scope.ServiceProvider.GetService<IHostedService>();
            return outboxCleanerHost;
        }

        public static IHostedService GetSpammerHost(string outputQueue)
        {
            var additionalOverrideConfig = new Dictionary<string, string>
            {
                ["rabbitMq:inputQueue"] = outputQueue, 
                ["rabbitMq:outputQueue"] = outputQueue
            };
            var provider = TestServiceProvider.GetSpammerServiceProvider(additionalOverrideConfig);
            var scope = provider.CreateScope();
            var spammerHost = scope.ServiceProvider.GetService<IHostedService>();
            return spammerHost;
        }
        
        public static IHostedService GetNServiceBusHost(string safeRebusInputQueue)
        {
            var additionalOverrideConfig = new Dictionary<string, string>
            {
                ["rabbitMq:outputQueue"] = safeRebusInputQueue
            };
            var provider = TestServiceProvider.GetNServiceBusServiceProvider(additionalOverrideConfig);
            var scope = provider.CreateScope();
            var nServiceBusHost = scope.ServiceProvider.GetService<IHostedService>();
            return nServiceBusHost;
        }
    }
}