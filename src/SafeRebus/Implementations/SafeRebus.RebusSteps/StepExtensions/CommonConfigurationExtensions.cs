using System;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Pipeline.Send;

namespace SafeRebus.RebusSteps.StepExtensions
{
    public static class CommonConfigurationExtensions
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
        
        public static OptionsConfigurer RegisterOutgoingStep(this OptionsConfigurer configurer, 
            IOutgoingStep stepToInject, 
            PipelineRelativePosition position = PipelineRelativePosition.Before, 
            Type anchorStep = null)
        {
            anchorStep = anchorStep ?? typeof(SendOutgoingMessageStep);
            configurer.Decorate<IPipeline>(c =>
            {
                var pipeline = c.Get<IPipeline>();

                return new PipelineStepInjector(pipeline)
                    .OnSend(stepToInject, position, anchorStep);
            });

            return configurer;
        }
    }
}