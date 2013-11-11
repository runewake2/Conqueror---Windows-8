using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConquerorClient
{
    public class NetworkAccess
    {
        public static event EventHandler ConnectionChanged;

        private static int _usingConnections = 0;

        public static void FinishConnection()
        {
            _usingConnections--;
            if (ConnectionChanged != null)
                ConnectionChanged(null, null);
        }

        public static void StartConnection()
        {
            _usingConnections++;
            if (ConnectionChanged != null)
                ConnectionChanged(null, null);
        }

        public static int Connections
        {
            get
            {
                return _usingConnections;
            }
        }
        
        public static bool UsingConnection
        {
            get
            {
                return _usingConnections > 0;
            }
        }
    }
}
