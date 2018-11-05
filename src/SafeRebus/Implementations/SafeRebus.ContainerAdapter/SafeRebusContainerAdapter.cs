using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Extensions;
using Rebus.Handlers;
using Rebus.Pipeline;
using Rebus.Transport;
using SafeRebus.Config;

namespace SafeRebus.ContainerAdapter
{
    public class SafeRebusContainerAdapter : IContainerAdapter
    {
        private readonly IServiceProvider ServiceProvider;
        
        private IBus Bus;

        public SafeRebusContainerAdapter(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        public Task<IEnumerable<IHandleMessages<TMessage>>> GetHandlers<TMessage>(TMessage message, ITransactionContext transactionContext)
        {
            var scope = transactionContext.GetOrAdd(SafeRebusContextTags.ScopeContextTag, () => ServiceProvider.CreateScope());
            var incomingStepContext = transactionContext.GetOrThrow<IncomingStepContext>(StepContext.StepContextKey);
            incomingStepContext.Save(SafeRebusContextTags.ScopeContextTag, scope);

            var resolvedHandlerInstances = GetMessageHandlersForMessage<TMessage>(scope);

            transactionContext.OnDisposed(scope.Dispose);

            return Task.FromResult((IEnumerable<IHandleMessages<TMessage>>)resolvedHandlerInstances.ToArray());
        }

        public void SetBus(IBus bus)
        {
            if (Bus != null)
            {
                throw new InvalidOperationException("Cannot set the bus instance more than once on the container adapter.");
            }

            Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
        
        private List<IHandleMessages<TMessage>> GetMessageHandlersForMessage<TMessage>(IServiceScope scope)
        {
            var handledMessageTypes = typeof(TMessage).GetBaseTypes()
                .Concat(new[] { typeof(TMessage) });

            return handledMessageTypes
                .SelectMany(t =>
                {
                    var implementedInterface = typeof(IHandleMessages<>).MakeGenericType(t);

                    return scope.ServiceProvider.GetServices(implementedInterface).Cast<IHandleMessages>();
                })
                .Cast<IHandleMessages<TMessage>>()
                .ToList();
        }
    }
}