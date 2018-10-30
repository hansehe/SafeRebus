using System.Threading.Tasks;

namespace SafeRebus.Outbox.Abstractions
{
    public interface IOutboxDuplicationFilterCleaner
    {
        Task CleanMessageIds();
    }
}