using System.Net;
using System.Net.Sockets;

namespace Network
{
    public class Server
    {
        Socket _listenSocket;

        public Server(int port)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint ipEndPoint = new(ipAddress, port);
            _listenSocket = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(ipEndPoint);
            _listenSocket.Listen(5);
        }
    }
}
