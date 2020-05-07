using System;

namespace MUpdate.Events
{
    public class AsyncUpdateFinishedEventArgs : EventArgs
    {
        public AsyncUpdateFinishedEventArgs(bool successful, Exception ex = null)
        {
            Successful = successful;
            Ex = ex;
        }

        public Exception Ex { get; private set; }
        public bool Successful { get; private set; }
    }
}