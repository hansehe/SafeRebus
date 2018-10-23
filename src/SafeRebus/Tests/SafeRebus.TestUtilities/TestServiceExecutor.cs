using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        
        private static IServiceProvider GetServiceProvider()
        {
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebus()
                .UseDefaultTestRebusConfiguration()
                .BuildServiceProvider();
            return provider;
        }
        
        private static IServiceCollection UseDefaultTestRebusConfiguration(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Config.Config.GetConfigFilename)
                .AddInMemoryCollection(GetOverrideConfig())
                .Build());
        }

        private static Dictionary<string, string> GetOverrideConfig()
        {
            var random = new Random();
            var randomQueue = $"SafeRebus.InputQueue_{random.Next().ToString()}";
            var overrideDict = new Dictionary<string, string>
            {
                {"rabbitMq:inputQueue", randomQueue},
                {"rabbitMq:outputQueue", randomQueue}
            };
            return overrideDict;
        }
    }
}