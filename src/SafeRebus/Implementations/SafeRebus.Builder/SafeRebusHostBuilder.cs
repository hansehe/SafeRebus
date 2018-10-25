using Microsoft.Extensions.Hosting;

namespace SafeRebus.Builder
{
    public static class SafeRebusHostBuilder
    {
        public static IHost BuildSafeRebusHostBuilder()
        {
            return new HostBuilder()
                .ConfigureServices(serviceCollection => serviceCollection.ConfigureWithSafeRebus())
                .Build();
        }
    }
}