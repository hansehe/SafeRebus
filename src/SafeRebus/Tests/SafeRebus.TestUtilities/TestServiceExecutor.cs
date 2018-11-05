using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;

namespace SafeRebus.TestUtilities
{
    public static class TestServiceExecutor
    {
        public static async Task ExecuteInScope(Func<IServiceScope, Task> action, 
            Dictionary<string, string> additionalOverrideConfig = null,
            IServiceProvider provider = null)
        {
            provider = provider ?? TestServiceProvider.GetMessageHandlerServiceProvider(additionalOverrideConfig);
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