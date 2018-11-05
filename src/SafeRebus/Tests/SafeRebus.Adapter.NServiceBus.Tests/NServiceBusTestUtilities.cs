using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Config;
using SafeRebus.TestUtilities;

namespace SafeRebus.Adapter.NServiceBus.Tests
{
    public static class NServiceBusTestUtilities
    {
        public static async Task ExecuteInNServiceBusScope(Func<IServiceScope, Task> action)
        {
            var nServiceBusIntegrationAdditionalConfig = new Dictionary<string, string>();
            var nServiceBusScope = TestServiceExecutor.GetNServiceBusServiceProvider(NServiceBusOverrideConfig.GetNServiceBusOverrideConfig()).CreateScope();
            
            var inputQueue = nServiceBusScope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
            var outputQueue = nServiceBusScope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();
            
            nServiceBusIntegrationAdditionalConfig["rabbitMq:inputQueue"] = outputQueue;
            nServiceBusIntegrationAdditionalConfig["rabbitMq:outputQueue"] = inputQueue;
            
            var rebusScope = TestServiceExecutor.GetServiceProvider(nServiceBusIntegrationAdditionalConfig).CreateScope();
            var bus = rebusScope.ServiceProvider.GetService<IBus>();

            try
            {
                await action.Invoke(nServiceBusScope);
            }
            finally
            {
                nServiceBusScope.ServiceProvider.GetService<IRabbitMqUtility>().DeleteInputQueue();
                rebusScope.ServiceProvider.GetService<IRabbitMqUtility>().DeleteInputQueue();
            }
        }
    }
}