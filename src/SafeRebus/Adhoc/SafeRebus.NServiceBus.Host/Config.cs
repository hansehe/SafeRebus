using Microsoft.Extensions.Configuration;
using SafeRebus.Config;

namespace SafeRebus.NServiceBus.Host
{
    public static class Config
    {
        public const string DefaultConfig = "DefaultNServiceBusConfig.json";

        private const string ConnectionStringTemplate = "host={0};username={1};password={2}";

        public static string GetNServiceBusConnectionString(this IConfiguration configuration)
        {
            var hostname = configuration.GetRabbitMqHostname();
            var username = configuration.GetRabbitMqUser();
            var password = configuration.GetRabbitMqPassword();
            var connectionString = string.Format(ConnectionStringTemplate,
                hostname,
                username,
                password);
            return connectionString;
        }
    }
}