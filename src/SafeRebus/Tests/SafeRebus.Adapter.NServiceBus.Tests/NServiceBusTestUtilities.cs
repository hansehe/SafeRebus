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
            var nServiceBusScope = TestServiceProvider.GetNServiceBusServiceProvider(
                NServiceBusOverrideConfig.GetNServiceBusAdditionalOverrideConfig())
                .CreateScope();
            
            var inputQueue = nServiceBusScope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqInputQueue();
            var outputQueue = nServiceBusScope.ServiceProvider.GetService<IConfiguration>().GetRabbitMqOutputQueue();

            var rebusIntegrationAdditionalConfig = new Dictionary<string, string>
            {
                ["rabbitMq:inputQueue"] = outputQueue, 
                ["rabbitMq:outputQueue"] = inputQueue
            };

            var rebusScope = TestServiceProvider.GetMessageHandlerServiceProvider(rebusIntegrationAdditionalConfig).CreateScope();
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