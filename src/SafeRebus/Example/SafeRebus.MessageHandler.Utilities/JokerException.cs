using System;
using SafeRebus.MessageHandler.Config;

namespace SafeRebus.MessageHandler.Utilities
{
    public static class JokerException
    {
        private static readonly Random Random = new Random();
        private static readonly object GlobalLock = new object();
        
        public static void MaybeThrowJokerException()
        {
            if (!JokerExceptionsConfig.UseJokerExceptions)
            {
                return;
            }
    
            int randomPercent;
            lock (GlobalLock)
            {
                randomPercent = Random.Next(100);                
            }
    
            if (randomPercent < JokerExceptionsConfig.JokerExceptionProbabilityInPercent)
            {
                throw new Exception("Joker exception thrown!");
            }
        }
    }
}