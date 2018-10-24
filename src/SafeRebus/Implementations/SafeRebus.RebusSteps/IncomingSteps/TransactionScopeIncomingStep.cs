using System;
using System.Threading.Tasks;
using System.Transactions;
using Rebus.Pipeline;
using Rebus.Transport;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class TransactionScopeIncomingStep : IIncomingStep
    {
        private readonly IServiceProvider ServiceProvider;
        
        private const TransactionScopeOption ScopeOption = TransactionScopeOption.Required;
        private const TransactionScopeAsyncFlowOption AsyncFlowOption = TransactionScopeAsyncFlowOption.Enabled;

        private TransactionOptions TransactionOptions => new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromMinutes(1)
        };

        public TransactionScopeIncomingStep(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            if (Transaction.Current == null)
            {
                using (var scope = new TransactionScope(ScopeOption, TransactionOptions, AsyncFlowOption))
                {
                    await next();
                    scope.Complete();
                }
            }
            else
            {
                await next();
            }
        }
    }
}