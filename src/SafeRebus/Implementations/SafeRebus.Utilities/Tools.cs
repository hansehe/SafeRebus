using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public static class Tools
    {
        private const long DefaultTimeoutMs = 20000;
        private const long DefaultCycleDelayMs = 50;
        private const int JokerExceptionProbabilityInPercent = 2;

        public static void MaybeThrowJokerException()
        {
            if (!BaseConfig.UseJokerExceptions)
            {
                return;
            }
            var random = new Random();
            if (random.Next(100) < JokerExceptionProbabilityInPercent)
            {
                throw new Exception("Joker exception thrown!");
            }
        }
        
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