using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Pipeline;
using SafeRebus.Abstractions;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class DbTransactionScopeIncomingStep : IIncomingStep
    {
        private readonly IServiceProvider ServiceProvider;

        public DbTransactionScopeIncomingStep(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var dbProvider = ServiceProvider.GetService<IDbProvider>();
            using (var transaction = dbProvider.GetDbTransaction())
            {
                try
                {
                    await next();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}