using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SafeRebus.Abstractions;
using SafeRebus.Config;
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
        
        public static async Task CleanOutbox(this IServiceProvider serviceProvider, ILogger logger)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var threshold = scope.ServiceProvider.GetService<IConfiguration>().GetHostCleanOldMessageIdsFromOutboxTimeThresholdSec();
                logger.LogInformation($"Cleaning outbox of message ids older then: {threshold.ToString()} seconds.");
                var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
                var dbProvider = scope.ServiceProvider.GetService<IDbProvider>();
                await outboxRepository.CleanOldMessageIds(threshold);
                dbProvider.GetDbTransaction().Commit();
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

        public static void StopTimer(Timer timer)
        {
            timer?.Change(Timeout.Infinite, 0);
            timer?.Dispose();
        }
    }
}