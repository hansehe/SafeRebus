using Microsoft.Extensions.Hosting;

namespace SafeRebus.MessageHandler.Builder
{
    public static class SafeRebusMessageHandlerHostBuilder
    {
        public static IHost BuildSafeRebusMessageHandlerHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebusMessageHandler())
                .Build();
        }
        
        public static IHost BuildSafeRebusOutboxCleanerHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebusOutboxCleanerHost())
                .Build();
        }
        
        public static IHost BuildSafeRebusSpammerHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebusMessageSpammer())
                .Build();
        }
    }
}