using System.Reflection;

namespace SafeRebus.Outbox.Migration
{
    public static class MigrationAssembly
    {
        public static Assembly GetMigrationAssembly => typeof(AddOutboxTable).Assembly;
    }
}