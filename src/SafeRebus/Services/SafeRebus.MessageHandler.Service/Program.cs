using Microsoft.Extensions.Hosting;
using SafeRebus.MessageHandler.Builder;

namespace SafeRebus.MessageHandler.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            SafeRebusMessageHandlerHostBuilder.BuildSafeRebusMessageHandlerHost()
                .Run();
        }
    }
}