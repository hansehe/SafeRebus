using System.Reflection;

namespace SafeRebus.MessageHandler.Migration
{
    public static class MigrationAssembly
    {
        public static Assembly GetMigrationAssembly => typeof(AddResponseTable).Assembly;
    }
}