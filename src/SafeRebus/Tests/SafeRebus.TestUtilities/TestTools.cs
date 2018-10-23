using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeRebus.Contracts;
using SafeRebus.Contracts.Requests;

namespace SafeRebus.TestUtilities
{
    public static class TestTools
    {
        private const int TimeoutMs = 5000;

        public static async Task WaitUntilSuccess(Func<Task> successFunc)
        {
            var timeout = TimeSpan.FromMilliseconds(TimeoutMs);
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
            }
        }

        public static SafeRebusRequest GetRequest()
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