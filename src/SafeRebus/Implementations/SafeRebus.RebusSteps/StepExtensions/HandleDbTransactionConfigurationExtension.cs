using System;
using Rebus.Config;
using SafeRebus.RebusSteps.IncomingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    internal static class HandleDbTransactionConfigurationExtension 
    {
        public static OptionsConfigurer HandleDbTransaction(this OptionsConfigurer configurer)
        {
            return configurer.RegisterIncomingStep(new HandleDbTransactionIncomingStep());
        }
    }
}