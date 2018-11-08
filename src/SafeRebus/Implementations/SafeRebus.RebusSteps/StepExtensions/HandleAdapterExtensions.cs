using System;
using Rebus.Config;
using Rebus.Pipeline.Receive;
using SafeRebus.RebusSteps.IncomingSteps;
using SafeRebus.RebusSteps.OutgoingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    public static class HandleAdapterExtensions
    {
        public static OptionsConfigurer HandleMessageWithSafeStandardAdapter(this OptionsConfigurer configurer, IServiceProvider serviceProvider)
        {
            return configurer
                .RegisterIncomingStep(new HandleSafeStandardAdapterIncomingStep(serviceProvider),
                    anchorStep: typeof(DeserializeIncomingMessageStep))
                .RegisterOutgoingStep(new HandleSafeStandardAdapterOutgoingStep(serviceProvider));
        }
    }
}