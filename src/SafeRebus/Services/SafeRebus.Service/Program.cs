using Microsoft.Extensions.Hosting;
using SafeRebus.Builder;

namespace SafeRebus.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            SafeRebusHostBuilder.BuildSafeRebusHostBuilder()
                .Run();
        }
    }
}