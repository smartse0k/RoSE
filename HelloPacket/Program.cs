﻿using Network;
using Util;

namespace HelloPacket
{
    public class Program
    {
        public class LoginRequest: Packet
        {
            public string id = "";
            public string password = "";

            protected override int OpCode => (int)ClientOpCode.LoginRequest;

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
                    case (int)ClientOpCode.LoginRequest:
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