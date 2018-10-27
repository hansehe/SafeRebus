using Microsoft.Extensions.Hosting;

namespace SafeRebus.Builder
{
    public static class SafeRebusHostBuilder
    {
        public static IHost BuildSafeRebusHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebus())
                .Build();
        }
        
        public static IHost BuildSafeRebusOutboxCleanerHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebusOutboxCleaner())
                .Build();
        }
        
        public static IHost BuildSafeRebusSpammerHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebusSpammer())
                .Build();
        }
    }
}