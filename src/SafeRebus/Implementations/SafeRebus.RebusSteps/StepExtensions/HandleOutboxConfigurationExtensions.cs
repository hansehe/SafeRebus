using Rebus.Config;
using SafeRebus.RebusSteps.IncomingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    internal static class HandleOutboxConfigurationExtensions 
    {
        public static OptionsConfigurer HandleMessageInOutboxTransaction(this OptionsConfigurer configurer)
        {
            return configurer.RegisterIncomingStep(new HandleMessageInOutboxTransactionIncomingStep());
        }
        
        public static OptionsConfigurer HandleMessageWithOutboxDuplicationFilter(this OptionsConfigurer configurer)
        {
            return configurer.RegisterIncomingStep(new HandleOutboxDuplicationFilterIncomingStep());
        }
    }
}