using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SafeRebus.Abstractions;
using SafeRebus.Contracts.Requests;

namespace SafeRebus.Host
{
    internal static class SafeRebusHostHelper
    {
        public static async Task AssertReceivedResponses(this IResponseRepository responseRepository, SafeRebusRequest[] requests)
        {
            var requestIds = requests.Select(request => request.Id).ToArray();
            var responseIds = (await responseRepository.SelectResponses(requestIds))
                .Select(response => response.Id).ToArray();
            foreach (var requestId in requestIds)
            {
                responseIds.Should().Contain(requestId);
            }
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

        public static async Task RunUntilCancelled(Func<Task> func, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await func.Invoke();
            }
        }
    }
}