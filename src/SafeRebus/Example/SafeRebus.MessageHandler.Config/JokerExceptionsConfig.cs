using System;

namespace SafeRebus.MessageHandler.Config
{
    public static class JokerExceptionsConfig
    {
        private const int DefaultJokerExceptionProbabilityInPercent = 1;
        
        public static bool UseJokerExceptions
        {
            get => Environment.GetEnvironmentVariable("USE_JOKER_EXCEPTIONS") == "true";
            set => Environment.SetEnvironmentVariable("USE_JOKER_EXCEPTIONS", value.ToString().ToLower());
        }
        
        public static int JokerExceptionProbabilityInPercent
        {
            get
            {
                var probPercentStr = Environment.GetEnvironmentVariable("JOKER_EXCEPTION_PROBABILITY_PERCENT");
                if (!int.TryParse(probPercentStr, out var probPercent))
                {
                    probPercent = DefaultJokerExceptionProbabilityInPercent;
                }
                return probPercent;
            }
            set => Environment.SetEnvironmentVariable("JOKER_EXCEPTION_PROBABILITY_PERCENT", value.ToString());
        }
    }
}