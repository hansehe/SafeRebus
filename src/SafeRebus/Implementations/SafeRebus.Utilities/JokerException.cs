using System;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public static class JokerException
    {
        private static readonly Random Random = new Random();
        private static readonly object GlobalLock = new object();
        private const int JokerExceptionProbabilityInPercent = 1;
        
        public static void MaybeThrowJokerException()
        {
            if (!BaseConfig.UseJokerExceptions)
            {
                return;
            }
    
            int randomPercent;
            lock (GlobalLock)
            {
                randomPercent = Random.Next(100);                
            }
    
            if (randomPercent < JokerExceptionProbabilityInPercent)
            {
                throw new Exception("Joker exception thrown!");
            }
        }
    }
}