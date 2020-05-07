using System;

namespace MUpdate.Events
{
    public static class MUpdateEventBridge
    {
        public static event EventHandler AsyncUpdateFinished;

        internal static void RaiseAsyncUpdateFinished(bool successful, Exception ex = null)
        {
            AsyncUpdateFinished?.Invoke(null, new AsyncUpdateFinishedEventArgs(successful, ex));
        }
    }
}