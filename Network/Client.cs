﻿using System.Net;
using System.Net.Sockets;

namespace Network
{
    public class Client
    {
        public static Task<Session> Connect(string destination, int port, Func<Socket, Session> createSession)
        {
            IPAddress ipAddress = IPAddress.Parse(destination);
            IPEndPoint ipEndPoint = new(ipAddress, port);

            Socket socket = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            return socket.ConnectAsync(ipEndPoint).ContinueWith((Task task) =>
            {
                task.Wait();
                return createSession(socket);
            });
        }
    }
}
