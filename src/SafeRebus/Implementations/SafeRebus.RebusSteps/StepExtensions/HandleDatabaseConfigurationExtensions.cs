using Rebus.Config;
using SafeRebus.RebusSteps.IncomingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    internal static class HandleDatabaseConfigurationExtensions 
    {
        public static OptionsConfigurer HandleMessageInDatabaseTransaction(this OptionsConfigurer configurer)
        {
            return configurer.RegisterIncomingStep(new HandleDatabaseTransactionIncomingStep());
        }
    }
}