using System;
using Rebus.Config;
using Rebus.Pipeline.Receive;
using SafeRebus.RebusSteps.IncomingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    public static class ExecutionScopeConfigurationExtension
    {
        public static OptionsConfigurer UseExecutionScope(this OptionsConfigurer configurer, IServiceProvider serviceProvider)
        {
            return configurer.RegisterIncomingStep(new ServiceProviderScopeIncomingStep(serviceProvider), anchorStep: typeof(DeserializeIncomingMessageStep));
        }
    }
}