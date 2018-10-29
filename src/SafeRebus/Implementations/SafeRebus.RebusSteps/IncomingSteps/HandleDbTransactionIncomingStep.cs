using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Pipeline;
using SafeRebus.Abstractions;
using SafeRebus.Config;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class HandleDbTransactionIncomingStep : IIncomingStep
    {
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var scope = context.Load<IServiceScope>(SafeRebusContextTags.ScopeContextTag);
            var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
            try
            {
                await next();
            }
            catch (Exception e)
            {
                dbProvider.GetDbTransaction().Rollback();
                throw;
            }
            dbProvider.GetDbTransaction().Commit();
        }
    }
}