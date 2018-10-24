using System;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using SafeRebus.RebusSteps.IncomingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    internal static class OutboxConfigurationExtension 
    {
        public static OptionsConfigurer HandleMessageWithOutboxPattern(this OptionsConfigurer configurer, IServiceProvider serviceProvider)
        {
            return configurer.RegisterIncomingStep(new HandleOutboxIncomingStep(serviceProvider));
        }
    }
}