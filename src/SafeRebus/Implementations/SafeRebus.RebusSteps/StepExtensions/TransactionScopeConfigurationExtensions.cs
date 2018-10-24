using System;
using Rebus.Config;
using SafeRebus.RebusSteps.IncomingSteps;

namespace SafeRebus.RebusSteps.StepExtensions
{
    public static class TransactionScopeConfigurationExtensions
    {
        public static OptionsConfigurer HandleMessagesInsideTransactionScope(this Rebus.Config.OptionsConfigurer configurer, IServiceProvider serviceProvider)
        {
            return configurer.RegisterIncomingStep(new TransactionScopeIncomingStep(serviceProvider));
        }
    }
}