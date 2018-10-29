using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;
using SafeRebus.Abstractions;
using SafeRebus.Config;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class HandleOutboxIncomingStep : IIncomingStep
    {
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var scope = context.Load<IServiceScope>(SafeRebusContextTags.ScopeContextTag);
            var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
            var messageId = context.Load<TransportMessage>().Headers[Headers.MessageId];
            var messageGuidId = Guid.Parse(messageId);
            if (await outboxRepository.TryInsertMessageId(messageGuidId))
            {
                await next();
            }
        }
    }
}