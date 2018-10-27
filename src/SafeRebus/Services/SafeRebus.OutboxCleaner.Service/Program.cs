using Microsoft.Extensions.Hosting;
using SafeRebus.Builder;

namespace SafeRebus.OutboxCleaner.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            SafeRebusHostBuilder.BuildSafeRebusOutboxCleanerHost()
                .Run();
        }
    }
}