using Rebus.Logging;

namespace SafeRebus.Config
{
    internal static class DefaultConfig
    {
        public static long DefaultRabbitMqTimeoutMs = 5000;
        public static string DefaultRabbitMqUser = "guest";
        public static string DefaultRabbitMqPassword = "guest";
        public static string DefaultRabbitMqHostname = "localhost";
        public static LogLevel DefaultRabbitMqLogLevel = LogLevel.Info;
        public static string DefaultRabbitMqInputQueue = "SafeRebus.InputQueue";
        public static string DefaultRabbitMqOutputQueue = "SafeRebus.OutputQueue";
    }
}