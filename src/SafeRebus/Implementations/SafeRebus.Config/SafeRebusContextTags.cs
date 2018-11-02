using Rebus.Transport;

namespace SafeRebus.Config
{
    public static class SafeRebusContextTags
    {
        public const string ScopeContextTag = "scope";
        public static string TransactionContextTag = typeof(ITransactionContext).FullName;
    }
}