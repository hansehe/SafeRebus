using System;
using System.Data;
using System.Threading.Tasks;
using SafeRebus.Abstractions;

namespace SafeRebus.Database
{
    public class DbExecutor : IDbExecutor
    {
        private readonly IDbProvider DbProvider;

        public DbExecutor(IDbProvider dbProvider)
        {
            DbProvider = dbProvider;
        }

        public void Execute(Action<IDbConnection> action)
        {
            action.Invoke(DbProvider.GetDbConnection());
        }

        public Task ExecuteAsync(Func<IDbConnection, Task> func)
        {
            return func.Invoke(DbProvider.GetDbConnection());
        }

        public T Select<T>(Func<IDbConnection, T> func)
        {
            return func.Invoke(DbProvider.GetDbConnection());
        }

        public Task<T> SelectAsync<T>(Func<IDbConnection, Task<T>> func)
        {
            return func.Invoke(DbProvider.GetDbConnection());
        }

        public void ExecuteInTransaction(Action<IDbConnection> action)
        {
            action.Invoke(DbProvider.GetDbTransaction().Connection);
        }

        public Task ExecuteInTransactionAsync(Func<IDbConnection, Task> func)
        {
            return func.Invoke(DbProvider.GetDbTransaction().Connection);
        }

        public T SelectInTransaction<T>(Func<IDbConnection, T> func)
        {
            return func.Invoke(DbProvider.GetDbTransaction().Connection);
        }

        public Task<T> SelectInTransactionAsync<T>(Func<IDbConnection, Task<T>> func)
        {
            return func.Invoke(DbProvider.GetDbTransaction().Connection);
        }
    }
}