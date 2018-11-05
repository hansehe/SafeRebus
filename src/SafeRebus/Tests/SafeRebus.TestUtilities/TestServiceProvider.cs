using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.MessageHandler.Builder;
using SafeRebus.NServiceBus.Host;

namespace SafeRebus.TestUtilities
{
    public static class TestServiceProvider
    {
        public static IServiceProvider GetMessageHandlerServiceProvider(Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMessageHandler(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetOutboxCleanerServiceProvider(Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusOutboxCleanerHost(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetSpammerServiceProvider(Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMessageSpammer(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetNServiceBusServiceProvider(Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var provider = new ServiceCollection()
                .ConfigureWithNServiceBusHost(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
        
        public static IServiceProvider GetMigrationServiceProvider(Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetOverrideConfig();
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMessageHandlerMigration(overrideConfig)
                .BuildServiceProvider();
            return provider;
        }
    }
}