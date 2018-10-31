using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using SafeRebus.MessageHandler.Builder;

namespace SafeRebus.MessageSpammer.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5)); // Wait for other services to create queue
            SafeRebusMessageHandlerHostBuilder.BuildSafeRebusSpammerHost()
                .Run();
        }
    }
}