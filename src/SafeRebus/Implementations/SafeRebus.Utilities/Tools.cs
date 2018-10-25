using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public static class Tools
    {
        private const long DefaultTimeoutMs = 5000;
        private const long DefaultCycleDelayMs = 200;
        private const int JokerExceptionProbabilityInPercent = 5;

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
                catch (Exception e)
                {
                    if (stopWatch.Elapsed > timeout)
                    {
                        throw;
                    }
                }
                await Task.Delay(cycleDelay);
            }
        }

        public static void MaybeThrowJokerException()
        {
            if (BaseConfig.DropJokerExceptions)
            {
                return;
            }
            var random = new Random();
            if (random.Next(100) < JokerExceptionProbabilityInPercent)
            {
                throw new Exception("Joker exception thrown!");
            }
        }
    }
}