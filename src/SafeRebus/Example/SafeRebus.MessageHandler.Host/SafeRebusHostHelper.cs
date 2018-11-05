using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Contracts.Requests;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.MessageHandler.Host
{
    public static class SafeRebusHostHelper
    {
        public static Task AssertReceivedResponses(this IResponseRepository responseRepository, SafeRebusRequest[] requests)
        {
            var requestIds = requests.Select(request => request.Id).ToArray();
            return responseRepository.AssertReceivedResponses(requestIds);
        }
        
        public static Task AssertReceivedResponses(this IResponseRepository responseRepository, SafeRebusResponse[] responses)
        {
            var requestIds = responses.Select(request => request.Id).ToArray();
            return responseRepository.AssertReceivedResponses(requestIds);
        }

        public static SafeRebusRequest[] GetRequests(int nRequests)
        {
            var requests = new List<SafeRebusRequest>();
            for (var i = 0; i < nRequests; i++)
            {
                requests.Add(new SafeRebusRequest());
            }
            return requests.ToArray();
        }
        
        public static SafeRebusResponse[] GetResponses(int nResponses)
        {
            var responses = new List<SafeRebusResponse>();
            for (var i = 0; i < nResponses; i++)
            {
                responses.Add(new SafeRebusResponse());
            }
            return responses.ToArray();
        }

        public static async Task RunUntilCancelled(Func<Task> func, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await func.Invoke();
            }
        }
        
        private static async Task AssertReceivedResponses(this IResponseRepository responseRepository, Guid[] responseIds)
        {
            try
            {
                var savedResponses = (await responseRepository.SelectResponses(responseIds));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var savedResponseIds = (await responseRepository.SelectResponses(responseIds))
                .Select(response => response.Id).ToArray();
            foreach (var responseId in responseIds)
            {
                savedResponseIds.Should().Contain(responseId);
            }
        }
    }
}