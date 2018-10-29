using Microsoft.Extensions.Hosting;
using SafeRebus.MessageHandler.Builder;

namespace SafeRebus.OutboxCleaner.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            SafeRebusMessageHandlerHostBuilder.BuildSafeRebusOutboxCleanerHost()
                .Run();
        }
    }
}