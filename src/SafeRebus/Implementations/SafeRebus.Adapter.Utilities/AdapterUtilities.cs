using System;

namespace SafeRebus.Adapter.Utilities
{
    public static class AdapterUtilities
    {
        public static void InvokeIfTrue(this Action action, bool shouldInvokeFunc)
        {
            if (shouldInvokeFunc)
            {
                action.Invoke();
            }
        }
    }
}