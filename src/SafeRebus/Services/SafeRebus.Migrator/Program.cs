using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Builder;

namespace SafeRebus.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                .ConfigureWithSafeRebusMigration()
                .BuildServiceProvider();
            
            using (var scope = provider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
                runner.MigrateUp();
            }
        }
    }
}