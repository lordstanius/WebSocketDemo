using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketManager;

namespace WebSocketDemo
{
    public class SocketHandler : WebSocketHandler
    {
        public override void OnOpened(Socket socket)
        {
            Console.WriteLine("Connected socket: " + socket.ID);

            Task.Run(async () =>
            {
                var rand = new Random(Environment.TickCount);
                while (true)
                {
                    string quote = (rand.NextDouble() * 100.0).ToString("0.00");
                    await socket.SendMessageAsync(quote);
                    Thread.Sleep(1000);
                }
            });
        }

        public override void OnClosed(Socket socket)
        {
            Console.WriteLine("Socket closed: " + socket.ID);
        }

        public override void OnMessage(Socket socket, string message)
        {
            Console.WriteLine("Message from client: " + message);
        }
    }
}