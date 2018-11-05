using Microsoft.Extensions.Hosting;

namespace SafeRebus.NServiceBus.Host
{
    public static class NServiceBusHostBuilder
    {
        public static IHost BuildNServiceBusHost()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithNServiceBusHost())
                .Build();
        }
    }
}