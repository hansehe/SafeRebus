using Microsoft.Extensions.Hosting;
using SafeRebus.NServiceBus.Host;

namespace SafeRebus.NServiceBus.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            NServiceBusHostBuilder.BuildNServiceBusHost()
                .Run();
        }
    }
}