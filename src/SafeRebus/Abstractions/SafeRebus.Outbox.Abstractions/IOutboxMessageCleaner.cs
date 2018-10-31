using System.Threading.Tasks;

namespace SafeRebus.Outbox.Abstractions
{
    public interface IOutboxMessageCleaner
    {
        Task CleanMessages(bool ignoreAndLogExceptions = true);
    }
}