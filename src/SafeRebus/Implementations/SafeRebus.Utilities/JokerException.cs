using System;
using SafeRebus.Config;

namespace SafeRebus.Utilities
{
    public static class JokerException
    {
        private static readonly Random Random = new Random();
        private static readonly object GlobalLock = new object();
        
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
    
            if (randomPercent < BaseConfig.JokerExceptionProbabilityInPercent)
            {
                throw new Exception("Joker exception thrown!");
            }
        }
    }
}