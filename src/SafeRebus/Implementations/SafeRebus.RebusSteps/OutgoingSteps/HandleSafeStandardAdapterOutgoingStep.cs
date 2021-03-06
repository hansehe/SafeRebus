using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
using SafeRebus.Adapters.Abstractions.Adapters;
using SafeRebus.Config;

namespace SafeRebus.RebusSteps.OutgoingSteps
{
    public class HandleSafeStandardAdapterOutgoingStep : IOutgoingStep
    {
        private readonly IServiceProvider ServiceProvider;

        public HandleSafeStandardAdapterOutgoingStep(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        public Task Process(OutgoingStepContext context, Func<Task> next)
        {
            var transactionContext = context.Load<ITransactionContext>(SafeRebusContextTags.TransactionContextTag);
            var scope = transactionContext.GetOrAdd(SafeRebusContextTags.ScopeContextTag, () => ServiceProvider.CreateScope());
            var adapter = scope.ServiceProvider.GetService<ISafeStandardAdapter>();
            var transportMessage = context.Load<TransportMessage>();
            transportMessage = adapter.AppendAdapterSpecificHeaders(transportMessage);
            context.Save(transportMessage);
            return next();
        }
    }
}