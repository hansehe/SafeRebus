using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;
using SafeRebus.Abstractions;

namespace SafeRebus.RebusSteps.IncomingSteps
{
    public class HandleOutboxIncomingStep : IIncomingStep
    {
        private readonly IServiceProvider ServiceProvider;

        public HandleOutboxIncomingStep(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var outboxRepository = ServiceProvider.GetService<IOutboxRepository>();
            var correlationId = context.Load<TransportMessage>().Headers[Headers.CorrelationId];
            var correlationGuidId = Guid.Parse(correlationId);
            if (!await outboxRepository.MessageCorrelationIdExists(correlationGuidId))
            {
                await outboxRepository.InsertMessageCorrelationId(correlationGuidId);
                await next();
            }
        }
    }
}