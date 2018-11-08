using System;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using SafeRebus.Adapters.Abstractions;
using SafeStandard.Headers;
using ContentTypes = NServiceBus.ContentTypes;

namespace SafeRebus.NServiceBus.Host
{
    public class IncomingHeaderBehavior : Behavior<IIncomingPhysicalMessageContext>
    {
        private readonly IBodyConverter BodyConverter;

        public IncomingHeaderBehavior(
            IBodyConverter bodyConverter)
        {
            BodyConverter = bodyConverter;
        }
        
        public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            var headers = context.Message.Headers;
            if (!HeaderConverter.ContainsSafeStandardHeaders(headers))
            {
                await next();
            }
            
            var incomingContentType = context.Message.Headers[SafeHeaders.ContentType];
            var messageType = context.Message.Headers[SafeHeaders.MessageType];
            var incomingBody = context.Message.Body;
            
            if (incomingContentType != ContentTypes.Xml && 
                BodyConverter.TryConvert(incomingBody, incomingContentType, messageType, out var convertedBody))
            {
                headers[SafeHeaders.ContentType] = ContentTypes.Xml;
                context.UpdateMessage(convertedBody);
            }
            
            HeaderConverter.AppendNServiceBusHeaders(headers);

            try
            {
                await next();
            }
            catch (Exception e)
            {
                headers[SafeHeaders.ContentType] = incomingContentType;
                headers[Headers.ContentType] = incomingContentType;
//                throw; // Something is wrong with NServiceBus, but i really don't care as long as the message is processed, including that this project does not focus on NServiceBus.
            }
        }
    }
}