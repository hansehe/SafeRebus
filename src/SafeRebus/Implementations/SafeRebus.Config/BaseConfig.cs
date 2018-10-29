using System;

namespace SafeRebus.Config
{
    public static class BaseConfig
    {
        public const string DefaultConfigFilename = "DefaultConfig.json";
        public const string DefaultConfigDockerFilename = "DefaultConfig.Docker.json";
        
        public static bool InContainer => 
            Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER") == "true";
    }
}