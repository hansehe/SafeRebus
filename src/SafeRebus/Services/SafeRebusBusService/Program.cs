using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Builder;

namespace SafeRebusBusService
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebus()
                .BuildServiceProvider();
            
            using (var scope = provider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<IRebusRunner>();
                runner.Run().Wait();
            }
        }
    }
}