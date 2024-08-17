using System.Net.Sockets;
using System.Text;
using Util;

namespace Network
{
    public class Program
    {

        static void Main(string[] args)
        {
            Logger.LogLevel = LogLevel.INFO;

            Server server = new Server(8888, OnClientConnected);
            Logger.Info("server started.");

            while (true)
            {
                ;
            }
        }

        static void OnClientConnected(Socket clientSocket)
        {
            Logger.Info($"client connected. remoteEndPoint: {clientSocket.RemoteEndPoint}");

            byte[] welcome = Encoding.UTF8.GetBytes("CLIENT hello");
            clientSocket.Send(welcome);
        }
    }
}
