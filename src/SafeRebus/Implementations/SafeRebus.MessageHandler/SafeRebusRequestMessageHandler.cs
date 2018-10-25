using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using SafeRebus.Contracts;
using SafeRebus.Contracts.Requests;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.MessageHandler
{
    public class SafeRebusRequestMessageHandler : IHandleMessages<SafeRebusRequest>
    {
        private readonly ILogger Logger;
        private readonly IBus Bus;

        public SafeRebusRequestMessageHandler(
            ILogger<SafeRebusRequestMessageHandler> logger,
            IBus bus)
        {
            Logger = logger;
            Bus = bus;
        }

        public Task Handle(SafeRebusRequest message)
        {
            Logger.LogDebug($"Received {typeof(SafeRebusRequest)}");
            var response = HandleRequest(message);
            return Bus.Reply(response);
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