using System.Net;
using System.Net.Sockets;

namespace Somewhere2.SystemService
{
    public static class NetworkHelper
    {
        public static int FindFreeTcpPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}