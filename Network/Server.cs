using System.Net;
using System.Net.Sockets;
using Util;

namespace Network
{
    public class Server
    {
        Socket _listenSocket;
        Action<Socket> _onClientConnected; // FIXME : 나중에 인터페이스화 해야함

        public Server(int port, Action<Socket> onClientConnected)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint ipEndPoint = new(ipAddress, port);
            _listenSocket = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(ipEndPoint);
            _listenSocket.Listen(5);

            _onClientConnected = onClientConnected;

            Logger.Info($"listening at port {port}...");

            Accept();
        }

        void Accept()
        {
            _listenSocket.AcceptAsync().ContinueWith(OnAccept);
        }

        void OnAccept(Task<Socket> task)
        {
            Socket clientSocket = task.Result;
            _onClientConnected(clientSocket);
            Accept();
        }
    }
}
