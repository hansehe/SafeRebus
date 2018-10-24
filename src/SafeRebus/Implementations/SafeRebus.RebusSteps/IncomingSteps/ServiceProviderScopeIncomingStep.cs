using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Pipeline;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class ServiceProviderScopeIncomingStep : IIncomingStep
    {
        private readonly IServiceProvider ServiceProvider;

        public ServiceProviderScopeIncomingStep(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            using (ServiceProvider.CreateScope())
            {
                await next();
            }
        }
    }
}