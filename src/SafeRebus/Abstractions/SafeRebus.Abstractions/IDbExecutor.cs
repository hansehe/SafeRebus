using System;
using System.Data;
using System.Threading.Tasks;

namespace SafeRebus.Abstractions
{
    public interface IDbExecutor
    {
        void Execute(Action<IDbConnection> action);
        Task ExecuteAsync(Func<IDbConnection, Task> func);
        T Select<T>(Func<IDbConnection, T> func);
        Task<T> SelectAsync<T>(Func<IDbConnection, Task<T>> func);
        void ExecuteInTransaction(Action<IDbConnection> action);
        Task ExecuteInTransactionAsync(Func<IDbConnection, Task> func);
        T SelectInTransaction<T>(Func<IDbConnection, T> func);
        Task<T> SelectInTransactionAsync<T>(Func<IDbConnection, Task<T>> func);
    }
}