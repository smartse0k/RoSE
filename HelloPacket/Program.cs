using Network;
using Util;

namespace HelloPacket
{
    public class Program
    {

        abstract public class Packet: Serializable
        {
            protected abstract OpCode OpCode { get; }

            public new byte[] Serialize()
            {
                byte[] original = base.Serialize();
                int packetLength = sizeof(int) + sizeof(int) + original.Length;
                byte[] result = new byte[packetLength];

                Array.Copy(BitConverter.GetBytes(packetLength), 0, result, 0, sizeof(int));
                Array.Copy(BitConverter.GetBytes(((int)OpCode)), 0, result, sizeof(int), sizeof(int));
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

            public static OpCode? GetOpCode(byte[] input)
            {
                if (input.Length < sizeof(int) + sizeof(int))
                {
                    return null;
                }

                int opCode = BitConverter.ToInt32(input, sizeof(int));

                // TODO : 유효한 Enum인지 검증 추가

                return (OpCode)opCode;
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

        public class LoginRequest: Packet
        {
            public string id = "";
            public string password = "";

            protected override OpCode OpCode => OpCode.LoginRequest;

            protected override void DeserializeImpl(ReadOnlySpan<byte> span)
            {
                Get(span, ref id);
                Get(span, ref password);
            }

            protected override void SerializeImpl()
            {
                Put(id);
                Put(password);
            }
        }

        static void Main(string[] args)
        {
            {
                Console.WriteLine("loginRequest Serialize 테스트:");

                LoginRequest loginRequest = new();
                loginRequest.id = "gamemaster";
                loginRequest.password = "mysecretP@ssw0rd";

                byte[] loginRequestPacket = loginRequest.Serialize();
                string loginRequestPacketString = PrettyConverter.ToHexString(loginRequestPacket);
                
                Console.WriteLine(loginRequestPacketString);
                // 26 00 00 00 00 00 00 00 0A 00 67 61 6D 65 6D 61 73 74 65 72 10 00 6D 79 73 65 63 72 65 74 50 40 73 73 77 30 72 64
            }

            Console.WriteLine();

            {
                Console.WriteLine("loginRequest Deserialize 테스트:");

                string loginRequestPacketString = "26 00 00 00 00 00 00 00 0A 00 67 61 6D 65 6D 61 73 74 65 72 10 00 6D 79 73 65 63 72 65 74 50 40 73 73 77 30 72 64";
                byte[] loginRequestPacket = PrettyConverter.HexToByteArray(loginRequestPacketString)!;

                int? packetLength = Packet.GetPacketLength(loginRequestPacket);
                if (packetLength == null)
                {
                    Console.WriteLine("Error: failed to get packet length.");
                    return;
                }

                byte[]? packetPayload = Packet.GetPayload(loginRequestPacket);
                if (packetPayload == null)
                {
                    Console.WriteLine("Error: failed to get packet payload.");
                    return;
                }
                Console.WriteLine($"Packet Payload: {PrettyConverter.ToHexString(packetPayload)}");

                switch (Packet.GetOpCode(loginRequestPacket))
                {
                    case OpCode.LoginRequest:
                        LoginRequest loginRequest = new();
                        loginRequest.Deserialize(packetPayload);

                        Console.WriteLine($"id: {loginRequest.id}"); // gamemaster
                        Console.WriteLine($"password: {loginRequest.password}"); // mysecretP@ssw0rd
                        break;

                    default:
                        Console.WriteLine("Error: failed to determine op code.");
                        return;
                }
            }
        }
    }
}