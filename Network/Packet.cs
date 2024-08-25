namespace Network
{
    abstract public class Packet : Serializable
    {
        protected abstract int OpCode { get; }

        public new byte[] Serialize()
        {
            byte[] original = base.Serialize();
            int packetLength = sizeof(int) + sizeof(int) + original.Length;
            byte[] result = new byte[packetLength];

            Array.Copy(BitConverter.GetBytes(packetLength), 0, result, 0, sizeof(int));
            Array.Copy(BitConverter.GetBytes(OpCode), 0, result, sizeof(int), sizeof(int));
            Array.Copy(original, 0, result, sizeof(int) + sizeof(int), original.Length);

            return result;
        }

        public static int? GetPacketLength(byte[] input)
        {
            if (input.Length < sizeof(int))
            {
                return null;
            }

            int length = BitConverter.ToInt32(input, 0);

            return length;
        }

        /**
         * 0은 유효하지 않은 값
         */
        public static int GetOpCode(byte[] input)
        {
            if (input.Length < sizeof(int) + sizeof(int))
            {
                return 0;
            }

            int opCode = BitConverter.ToInt32(input, sizeof(int));
            return opCode;
        }

        public static byte[]? GetPayload(byte[] input)
        {
            int? length = GetPacketLength(input);
            if (length == null)
            {
                return null;
            }

            if (input.Length < length.Value)
            {
                return null;
            }

            int headerLength = sizeof(int) + sizeof(int);
            int payloadLength = length.Value - headerLength;

            byte[] result = new byte[payloadLength];
            Array.Copy(input, headerLength, result, 0, payloadLength);
            return result;
        }
    }
}
