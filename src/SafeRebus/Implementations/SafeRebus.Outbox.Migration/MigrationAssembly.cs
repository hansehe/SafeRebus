using System.Reflection;

namespace SafeRebus.Migration.Outbox
{
    public static class MigrationAssembly
    {
        public static Assembly GetMigrationAssembly => typeof(AddOutboxTable).Assembly;
    }
}