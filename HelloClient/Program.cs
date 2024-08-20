using System.Net.Sockets;
using System.Text;
using Network;
using Util;

namespace HelloClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            Logger.LogLevel = LogLevel.INFO;

            Logger.Info("wait for connect...");

            HelloSession session = (HelloSession)Client.Connect("127.0.0.1", 8888, CreateSession).Result;

            while (true)
            {
                Console.Write("input> ");

                string? input = Console.ReadLine()?.Trim();
                if (input == null || input.Length == 0)
                {
                    break;
                }

                byte[] data = Encoding.UTF8.GetBytes(input);
                session.Send(data);
            }
        }

        static Session CreateSession(Socket s)
        {
            Session session = new HelloSession(s);
            return session;
        }
    }
}