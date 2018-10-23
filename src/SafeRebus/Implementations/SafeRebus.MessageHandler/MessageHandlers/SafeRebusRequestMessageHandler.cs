using System;
using System.Threading.Tasks;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Contracts;
using SafeRebus.Contracts.Requests;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.MessageHandler.MessageHandlers
{
    public class SafeRebusRequestMessageHandler : MessageHandlerBase<SafeRebusRequest>
    {
        public override Task Handle<TMessage>(IBus bus, TMessage message)
        {
            Console.WriteLine($"Received {typeof(SafeRebusRequest)}");
            var response = HandleRequest(message as SafeRebusRequest);
            return bus.Reply(response);
        }

        private static SafeRebusResponse HandleRequest(SafeRebusRequest request)
        {
            var response = new SafeRebusResponse
            {
                Id = request.Id,
                RequestEnum = request.RequestEnum,
                Response = ResolveEnumResponse(request.RequestEnum)
            };
            return response;
        }

        private static string ResolveEnumResponse(RequestEnum requestEnum)
        {
            switch (requestEnum)
            {
                case RequestEnum.GiveMeYo:
                    return "Yo!";
                case RequestEnum.GiveMeHello:
                    return "Hello World!";
                case RequestEnum.GiveMeATotallyRandomSlogan:
                    return "This should be a totally random slogan..";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}