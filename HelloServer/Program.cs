using System.Net.Sockets;
using Network;
using Util;

namespace HelloServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Logger.LogLevel = LogLevel.INFO;

            Server server = new(8888, CreateSession);
            Logger.Info("server started.");

            while (true)
            {
                ;
            }
        }

        static Session CreateSession(Socket s)
        {
            Session session = new(s);
            return session;
        }
    }
}