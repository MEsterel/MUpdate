using System;
using System.Net;
using System.Runtime.InteropServices;

namespace MUpdate
{
    internal static class Tools
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        internal static bool CheckForInternetConnection()
        {
            try
            {
                int description;
                return InternetGetConnectedState(out description, 0);
            }
            catch
            {
                return false;
            }
        }
    }
}