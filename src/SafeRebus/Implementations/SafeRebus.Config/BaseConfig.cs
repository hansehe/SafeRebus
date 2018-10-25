using System;
using Rebus.Logging;

namespace SafeRebus.Config
{
    public static class BaseConfig
    {
        public const string DefaultConfigFilename = "DefaultConfig.json";
        public const string DefaultConfigDockerFilename = "DefaultConfig.Docker.json";
        
        public static bool InContainer => 
            Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER") == "true";

        public static bool DropJokerExceptions
        {
            get => Environment.GetEnvironmentVariable("DROP_JOKER_EXCEPTIONS") == "true";
            set
            {
                Environment.SetEnvironmentVariable("DROP_JOKER_EXCEPTIONS", value.ToString());
            }
        }

    }
}