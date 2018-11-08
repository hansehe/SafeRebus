using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
using SafeRebus.Adapters.Abstractions.Adapters;
using SafeRebus.Config;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class HandleSafeStandardAdapterIncomingStep : IIncomingStep
    {
        private readonly IServiceProvider ServiceProvider;

        public HandleSafeStandardAdapterIncomingStep(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var transactionContext = context.Load<ITransactionContext>(SafeRebusContextTags.TransactionContextTag);
            var scope = transactionContext.GetOrAdd(SafeRebusContextTags.ScopeContextTag, () => ServiceProvider.CreateScope());
            var adapter = scope.ServiceProvider.GetService<ISafeStandardAdapter>();
            var transportMessage = context.Load<TransportMessage>();
            if (adapter.IsUsableOnIncoming(transportMessage))
            {
                transportMessage = adapter.ConvertIncomingTransportMessage(transportMessage);
                context.Save(transportMessage);
            }

            await next();
        }
    }
}