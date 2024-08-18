using System.Net;
using System.Net.Sockets;
using Util;

namespace Network
{
    public class Server
    {
        readonly Socket _listenSocket;
        readonly Func<Socket, Session> _createSession;

        public Server(int port, Func<Socket, Session> createSession)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint ipEndPoint = new(ipAddress, port);
            _listenSocket = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(ipEndPoint);
            _listenSocket.Listen(5);

            _createSession = createSession;

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

            Session session = _createSession(clientSocket);
            session.OnConnected();
            session.StartReceive();

            Accept();
        }
    }
}
