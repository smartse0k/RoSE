using System.Net.Sockets;
using System.Text;
using Util;

namespace Network
{
    public class Session
    {
        readonly Socket _socket;
        bool _isConnected;

        // receive buffer
        const int ReceiveBufferSize = 1024 * 1024 * 4;
        readonly ArraySegment<byte> _receiveBuffer;
        int _receiveBufferWriteOffset;
        int _receiveBufferReadOffset;

        public Session(Socket socket)
        {
            _socket = socket;
            _isConnected = true;

            _receiveBuffer = new(new byte[ReceiveBufferSize], 0, ReceiveBufferSize);
            _receiveBufferWriteOffset = 0;

            OnConnected();
            StartReceive();
        }

        private void StartReceive()
        {
            if (_receiveBuffer.Array == null)
            {
                Logger.Error("[StartReceive] array of receive buffer is null.");
                return;
            }

            ArraySegment<byte> buffer = new(_receiveBuffer.Array, _receiveBufferWriteOffset, ReceiveBufferSize - _receiveBufferWriteOffset);
            _socket.ReceiveAsync(buffer, SocketFlags.None).ContinueWith(ContinueReceive);
        }

        public void OnConnected()
        {
            Logger.Info($"client connected. remoteEndPoint: {_socket.RemoteEndPoint}");

            byte[] welcome = Encoding.UTF8.GetBytes("CLIENT hello");
            _socket.Send(welcome);
        }

        private void ContinueReceive(Task<int> task)
        {
            int receivedBytes = task.Result;

            if (receivedBytes == 0)
            {
                _isConnected = false;
                OnDisconnected();
                return;
            }

            if (_receiveBuffer.Array == null)
            {
                Logger.Error("[ContinueReceive] array of receive buffer is null.");
                return;
            }

            _receiveBufferWriteOffset += receivedBytes;
            ArraySegment<byte> receiveBuffer = new(_receiveBuffer.Array, _receiveBufferReadOffset, _receiveBufferWriteOffset - _receiveBufferReadOffset);
            _receiveBufferReadOffset += OnReceive(receiveBuffer);

            StartReceive();
        }

        public int OnReceive(ArraySegment<byte> receiveBuffer)
        {
            string hexString = PrettyConverter.ToHexString(receiveBuffer);
            Logger.Info($"[OnReceive] receiveBuffer: {hexString}");
            return 0;
        }

        public void OnDisconnected()
        {
            Logger.Info($"client disconnected. remoteEndPoint: {_socket.RemoteEndPoint}");
        }
    }
}
