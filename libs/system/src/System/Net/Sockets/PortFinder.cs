using System.Net;
using System.Net.Sockets;

namespace Logicality.System.Net.Sockets
{
    public static class PortFinder
    {
        /// <summary>
        /// Let the OS assign the next available port. The OS will always increment the port number
        /// when making these calls which mitigates against races in parallel invocations runs where a
        /// multiple consumers are trying to bind to a port.
        /// </summary>
        /// <returns></returns>
        public static int GetNext()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)socket.LocalEndPoint).Port;
        }

    }
}
