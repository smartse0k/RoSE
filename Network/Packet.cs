using System.Text;

namespace Network
{
    abstract public class Packet
    {
        abstract public byte[] Serialize();
        abstract protected void DeserializeImpl(ReadOnlySpan<byte> span);

        // Serialize
        private const int SerializeBufferSize = 1024 * 1024 * 4;
        private byte[]? _serializeBuffer = null;
        private int _serializeWriteOffset = 0;

        private byte[] CheckAndExpandSerializeBuffer(int reserveSize)
        {
            if (_serializeBuffer == null)
            {
                _serializeBuffer = new byte[SerializeBufferSize];
            }
            int remainSize = _serializeBuffer.Length - _serializeWriteOffset;
            if (reserveSize > remainSize)
            {
                // 한번에 들어오려는 data의 크기가 SerializeBufferSize보단 작아야 함.
                byte[] newBuffer = new byte[_serializeBuffer.Length + SerializeBufferSize];
                Array.Copy(_serializeBuffer, newBuffer, _serializeWriteOffset);
                _serializeBuffer = newBuffer;
            }
            return _serializeBuffer;
        }

        protected void Put(byte[] data)
        {
            byte[] serializeBuffer = CheckAndExpandSerializeBuffer(data.Length);
            Array.Copy(data, 0, serializeBuffer, _serializeWriteOffset, data.Length);
            _serializeWriteOffset += data.Length;
        }

        protected void Put(bool data)
        {
            byte[] serializeBuffer = CheckAndExpandSerializeBuffer(1);
            if (data)
            {
                serializeBuffer[_serializeWriteOffset] = 1;
            } else {
                serializeBuffer[_serializeWriteOffset] = 0;
            }
            _serializeWriteOffset += 1;
        }

        protected void Put(short data)
        {
            Put(BitConverter.GetBytes(data));
        }

        protected void Put(int data)
        {
            Put(BitConverter.GetBytes(data));
        }

        protected void Put(string data)
        {
            byte[] stringData = Encoding.UTF8.GetBytes(data);
            short stringSize = (short)stringData.Length;
            Put(stringSize);
            Put(stringData);
        }

        // Deserialize
        private int _deserializeReadOffset = 0;

        protected void Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data;
            _deserializeReadOffset = 0;
            DeserializeImpl(span);
        }

        private bool CheckDeserializeBuffer(ReadOnlySpan<byte> span, int readSize)
        {
            int remainSize = span.Length - _deserializeReadOffset;
            if (readSize > remainSize)
            {
                return false;
            }

            return true;
        }

        protected bool Get(ReadOnlySpan<byte> span, ref bool target)
        {
            if (!CheckDeserializeBuffer(span, sizeof(bool)))
            {
                return false;
            }

            target = span.Slice(_deserializeReadOffset, sizeof(bool))[0] == 1;
            _deserializeReadOffset += sizeof(bool);

            return true;
        }

        protected bool Get(ReadOnlySpan<byte> span, ref short target)
        {
            if (!CheckDeserializeBuffer(span, sizeof(short)))
            {
                return false;
            }

            target = BitConverter.ToInt16(span.Slice(_deserializeReadOffset, sizeof(short)));
            _deserializeReadOffset += sizeof(short);

            return true;
        }

        protected bool Get(ReadOnlySpan<byte> span, ref int target)
        {
            if (!CheckDeserializeBuffer(span, sizeof(int)))
            {
                return false;
            }

            target = BitConverter.ToInt16(span.Slice(_deserializeReadOffset, sizeof(int)));
            _deserializeReadOffset += sizeof(int);

            return true;
        }

        protected bool Get(ReadOnlySpan<byte> span, ref string target)
        {
            if (!CheckDeserializeBuffer(span, sizeof(short)))
            {
                return false;
            }

            short stringSize = BitConverter.ToInt16(span.Slice(_deserializeReadOffset, sizeof(short)));
            _deserializeReadOffset += sizeof(short);

            if (!CheckDeserializeBuffer(span, stringSize))
            {
                return false;
            }

            target = Encoding.UTF8.GetString(span.Slice(_deserializeReadOffset, stringSize));

            return true;
        }
    }
}
