using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rebus.Bus;
using SafeRebus.Abstractions;
using SafeRebus.Contracts;
using SafeRebus.Contracts.Requests;

namespace SafeRebus.Runner
{
    public class SafeRebusRunner : IRebusRunner
    {
        private const long PauseBetweenRequestsMs = 500;
        private const int RequestsPerCycle = 10;
        
        private readonly IBus Bus;

        public SafeRebusRunner(
            IBus bus)
        {
            Bus = bus;
        }

        public async Task Run()
        {
            while (true)
            {
                for (var i = 0; i < RequestsPerCycle; i++)
                {
                    await SendRequest();
                }
                Thread.Sleep(TimeSpan.FromMilliseconds(PauseBetweenRequestsMs));
            }
        }

        private async Task SendRequest()
        {
            var request = GetRequest();
            await Bus.Send(request);
        }

        private static SafeRebusRequest GetRequest()
        {
            return new SafeRebusRequest
            {
                RequestEnum = GetRandomRequestEnum()
            };
        }

        private static RequestEnum GetRandomRequestEnum()
        {
            var random = new Random();
            var enumValues = Enum.GetValues(typeof(RequestEnum));
            var randomEnum = (RequestEnum)enumValues.GetValue(random.Next(enumValues.Length));
            return randomEnum;
        }
    }
}