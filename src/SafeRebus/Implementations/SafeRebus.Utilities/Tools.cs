using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SafeRebus.Utilities
{
    public static class Tools
    {
        public static async Task TriggerEveryCycle(Func<Task> triggerFunc, 
            TimeSpan triggerCycle,
            CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (stopWatch.Elapsed < triggerCycle)
                {
                    continue;
                }
                await triggerFunc.Invoke();
                stopWatch.Reset();
                stopWatch.Start();
            }
        }
    }
}