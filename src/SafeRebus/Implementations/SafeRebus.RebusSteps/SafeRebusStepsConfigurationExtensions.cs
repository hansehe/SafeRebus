using System;
using Rebus.Config;
using SafeRebus.RebusSteps.StepExtensions;

namespace SafeRebus.RebusSteps
{
    public static class SafeRebusStepsConfigurationExtensions
    {
        public static OptionsConfigurer HandleSafeRebusSteps(this OptionsConfigurer configurer, IServiceProvider serviceProvider)
        {
            // The steps are executed in the order they are registered.
            return configurer
                .HandleMessageWithSafeStandardAdapter(serviceProvider)
                .HandleMessageInOutboxTransaction()
                .HandleMessageInDatabaseTransaction()
                .HandleMessageWithOutboxDuplicationFilter();
        }
    }
}