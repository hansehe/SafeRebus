using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
using SafeRebus.Config;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class HandleMessageInOutboxTransactionIncomingStep : IIncomingStep
    {
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var scope = context.Load<IServiceScope>(SafeRebusContextTags.ScopeContextTag);
            var outboxBus = scope.ServiceProvider.GetService<IOutboxBus>();
            var transportMessage = context.Load<TransportMessage>();
            await outboxBus.BeginTransaction(transportMessage);
            await next();
            await outboxBus.Commit();
        }
    }
}