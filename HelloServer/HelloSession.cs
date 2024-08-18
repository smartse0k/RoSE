using Network;
using System.Net.Sockets;
using System.Text;
using Util;

namespace HelloServer
{
    public class HelloSession : Session
    {
        public HelloSession(Socket socket) : base(socket)
        {
        }

        public override void OnConnected()
        {
            Logger.Info($"client connected. remoteEndPoint: {_socket.RemoteEndPoint}");
        }

        public override void OnDisconnected()
        {
            Logger.Info($"client disconnected. remoteEndPoint: {_socket.RemoteEndPoint}");
        }

        public override int OnReceive(ArraySegment<byte> receiveBuffer)
        {
            string incoming = Encoding.UTF8.GetString(receiveBuffer.Array!, receiveBuffer.Offset, receiveBuffer.Count);
            Logger.Info($"data received. data: {incoming}, size: {receiveBuffer.Count}");

            byte[] outgoing = Encoding.UTF8.GetBytes($"Welcome {incoming}!");
            _socket.Send(outgoing);
            return receiveBuffer.Count;
        }
    }
}
