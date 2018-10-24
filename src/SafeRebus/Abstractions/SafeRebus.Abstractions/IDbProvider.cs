using System;
using System.Data;

namespace SafeRebus.Abstractions
{
    public interface IDbProvider : IDisposable
    {
        string GetConnectionString();
        IDbConnection GetDbConnection();
        IDbTransaction GetDbTransaction();
    }
}