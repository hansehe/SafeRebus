using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SafeRebus.Utilities
{
    public static class Tools
    {
        private const long DefaultTimeoutMs = 20000;
        private const long DefaultCycleDelayMs = 50;
        
        public static async Task WaitUntilSuccess(Func<Task> successFunc, 
            long timeoutMs = DefaultTimeoutMs, 
            long cycleDelayMs = DefaultCycleDelayMs)
        {
            var timeout = TimeSpan.FromMilliseconds(timeoutMs);
            var cycleDelay = TimeSpan.FromMilliseconds(cycleDelayMs);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                try
                {
                    await successFunc.Invoke();
                    break;
                }
                catch
                {
                    if (stopWatch.Elapsed > timeout)
                    {
                        throw;
                    }
                }
                await Task.Delay(cycleDelay);
            }
        }
    }
}