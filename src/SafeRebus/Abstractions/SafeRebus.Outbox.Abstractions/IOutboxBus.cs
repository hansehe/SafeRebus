using System.Threading.Tasks;
using Rebus.Bus;

namespace SafeRebus.Outbox.Abstractions
{
    public interface IOutboxBus : IBus
    {
        Task Commit();
        Task Rollback();
    }
}