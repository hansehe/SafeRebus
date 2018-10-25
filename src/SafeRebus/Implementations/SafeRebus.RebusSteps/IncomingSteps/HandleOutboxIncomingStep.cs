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
            using (var scope = ServiceProvider.CreateScope())
            {
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                var messageId = context.Load<TransportMessage>().Headers[Headers.MessageId];
                var messageGuidId = Guid.Parse(messageId);
                if (await outboxRepository.TryInsertMessageId(messageGuidId))
                {
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
    }
}