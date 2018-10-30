using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;
using SafeRebus.Config;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class HandleOutboxDuplicationFilterIncomingStep : IIncomingStep
    {
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var scope = context.Load<IServiceScope>(SafeRebusContextTags.ScopeContextTag);
            var outboxDuplicationFilterRepository = scope.ServiceProvider.GetService<IOutboxDuplicationFilterRepository>();
            var transportMessage = context.Load<TransportMessage>();
            var messageId = transportMessage.Headers[Headers.MessageId];
            var messageGuidId = Guid.Parse(messageId);
            if (await outboxDuplicationFilterRepository.TryInsertMessageId(messageGuidId))
            {
                await next();
            }
        }
    }
}