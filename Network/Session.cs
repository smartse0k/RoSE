using System.Net.Sockets;
using System.Text;
using Util;

namespace Network
{
    public class Session
    {
        readonly Socket _socket;

        public Session(Socket socket)
        {
            _socket = socket;
        }

        public void StartReceive()
        {

        }

        public void OnConnected()
        {
            Logger.Info($"client connected. remoteEndPoint: {_socket.RemoteEndPoint}");

            byte[] welcome = Encoding.UTF8.GetBytes("CLIENT hello");
            _socket.Send(welcome);
        }
    }
}
