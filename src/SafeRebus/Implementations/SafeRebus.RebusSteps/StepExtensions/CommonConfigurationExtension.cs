using System;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;

namespace SafeRebus.RebusSteps.StepExtensions
{
    public static class CommonConfigurationExtension
    {
        public static OptionsConfigurer RegisterIncomingStep(this OptionsConfigurer configurer, 
            IIncomingStep stepToInject, 
            PipelineRelativePosition position = PipelineRelativePosition.Before, 
            Type anchorStep = null)
        {
            anchorStep = anchorStep ?? typeof(DispatchIncomingMessageStep);
            configurer.Decorate<IPipeline>(c =>
            {
                var pipeline = c.Get<IPipeline>();

                return new PipelineStepInjector(pipeline)
                    .OnReceive(stepToInject, position, anchorStep);
            });

            return configurer;
        }
    }
}