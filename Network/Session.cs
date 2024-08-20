using System.Net.Sockets;
using System.Text;
using Util;

namespace Network
{
    public abstract class Session
    {
        protected readonly Socket _socket;
        public bool Connected { get; private set; }

        // receive buffer
        const int ReceiveBufferSize = 1024 * 1024 * 4;
        readonly ArraySegment<byte> _receiveBuffer;
        int _receiveBufferWriteOffset;
        int _receiveBufferReadOffset;

        public Session(Socket socket)
        {
            _socket = socket;
            Connected = true;

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

        private void ContinueReceive(Task<int> task)
        {
            int receivedBytes = 0;
            try
            {
                receivedBytes = task.Result;
            } catch (AggregateException ex) {
                if (ex.InnerException != null)
                {
                    Logger.Warn($"[ContinueReceive] {ex.InnerException.Message}");
                } else {
                    Logger.Warn($"[ContinueReceive] {ex.Message}");
                }
            }

            if (receivedBytes == 0)
            {
                Connected = false;
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

        public int Send(byte[] data)
        {
            return _socket.Send(data);
        }

        abstract public void OnConnected();

        abstract public int OnReceive(ArraySegment<byte> receiveBuffer);

        abstract public void OnDisconnected();
    }
}
