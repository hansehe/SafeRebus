using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SafeRebus.MessageHandler.Utilities
{
    public static class Tools
    {
        private const long DefaultTimeoutMs = 20000;
        private const long DefaultCycleDelayMs = 50;
        
        public static async Task WaitUntilSuccess(Func<Task> successFunc, 
            TimeSpan timeout = default(TimeSpan), 
            TimeSpan cycleDelay = default(TimeSpan))
        {
            timeout = timeout == default(TimeSpan) ? TimeSpan.FromMilliseconds(DefaultTimeoutMs) : timeout;
            cycleDelay = cycleDelay == default(TimeSpan) ? TimeSpan.FromMilliseconds(DefaultCycleDelayMs) : cycleDelay;
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