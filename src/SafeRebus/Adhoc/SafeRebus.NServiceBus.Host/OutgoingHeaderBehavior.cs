using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

namespace SafeRebus.NServiceBus.Host
{
    public class OutgoingHeaderBehavior : Behavior<IOutgoingPhysicalMessageContext>
    {
        public override Task Invoke(IOutgoingPhysicalMessageContext context, Func<Task> next)
        {
            HeaderConverter.AppendSafeStandardHeaders(context.Headers);
            return next();
        }
    }
}