using Microsoft.Extensions.Hosting;
using SafeRebus.Builder;

namespace SafeRebus.Spammer.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            SafeRebusHostBuilder.BuildSafeRebusSpammerHost()
                .Run();
        }
    }
}