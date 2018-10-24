using Microsoft.Extensions.Configuration;

namespace SafeRebus.Config
{
    public static class DbConfig
    {
        private static long DefaultDbTimeoutMs = 5000;
        private static string DefaultDbUser = "admin";
        private static string DefaultDbPassword = "admin";
        private static string DefaultDbHostname = "localhost";
        private static string DefaultDbPort = "5432";
        private static string DefaultDbName = "postgres";
        private static string DefaultDbPooling = "false";
        private static string DefaultDbSchema = "saferebus";
        
        public static long GetDbTimeoutMs(this IConfiguration configuration)
        {
            if (!long.TryParse(configuration["database:timeoutMs"], out var timeoutMs))
            {
                timeoutMs = DefaultDbTimeoutMs;
            }
            return timeoutMs;
        }
        
        public static string GetDbUser(this IConfiguration configuration)
        {
            return configuration["database:user"] ?? DefaultDbUser;
        }
        
        public static string GetDbPassword(this IConfiguration configuration)
        {
            return configuration["database:password"] ?? DefaultDbPassword;
        }
        
        public static string GetDbHostname(this IConfiguration configuration)
        {
            return configuration["database:hostname"] ?? DefaultDbHostname;
        }
        
        public static string GetDbPort(this IConfiguration configuration)
        {
            return configuration["database:port"] ?? DefaultDbPort;
        }
        
        public static string GetDbName(this IConfiguration configuration)
        {
            return configuration["database:databaseName"] ?? DefaultDbName;
        }
        
        public static string GetDbPooling(this IConfiguration configuration)
        {
            return configuration["database:pooling"] ?? DefaultDbPooling;
        }
        
        public static string GetDbSchema(this IConfiguration configuration)
        {
            return (configuration["database:schema"] ?? DefaultDbSchema).ToLower();
        }
    }
}