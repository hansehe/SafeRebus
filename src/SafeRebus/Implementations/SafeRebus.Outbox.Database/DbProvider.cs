using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SafeRebus.Abstractions;
using SafeRebus.Config;

namespace SafeRebus.Database.Outbox
{
    public class DbProvider : IDbProvider
    {
        private readonly IConfiguration Configuration;
        private const string ConnectionStringTemplate = "User ID={0};Password={1};Host={2};Port={3};Database={4};Pooling={5};";

        private IDbConnection DbConnection;
        private IDbTransaction DbTransaction;

        private string User => Configuration.GetDbUser();
        private string Password => Configuration.GetDbPassword();
        private string Host => Configuration.GetDbHostname();
        private string Port => Configuration.GetDbPort();
        private string Database => Configuration.GetDbName();
        private string Pooling => Configuration.GetDbPooling();
        
        public DbProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetConnectionString()
        {
            var connectionString = string.Format(ConnectionStringTemplate,
                User,
                Password,
                Host,
                Port,
                Database,
                Pooling);
            return connectionString;
        }

        public IDbConnection GetDbConnection()
        {
            DbConnection = DbConnection ?? CreateAndOpenDbConnection();
            return DbConnection;
        }

        public IDbTransaction GetDbTransaction()
        {
            DbTransaction = DbTransaction ?? GetDbConnection().BeginTransaction();
            return DbTransaction;
        }

        private IDbConnection CreateAndOpenDbConnection()
        {
            var dbConnection = new NpgsqlConnection(GetConnectionString());
            dbConnection.Open();
            return dbConnection;
        }

        public void Dispose()
        {
            DbTransaction?.Dispose();
            DbConnection?.Dispose();
        }
    }
}