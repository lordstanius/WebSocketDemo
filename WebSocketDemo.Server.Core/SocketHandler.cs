using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebSocketManager;
using WebSocketManager.Common;

namespace WebSocketDemo
{
    public class SocketHandler : WebSocketHandler
    {
        public SocketHandler(WebSocketConnectionManager webSocketConnectionManager): base(webSocketConnectionManager)
        {
        }

        public override Task OnConnected(WebSocket socket)
        {
            System.Diagnostics.Debug.WriteLine("Connected socket: " + WebSocketConnectionManager.GetId(socket));

            Task.Run(() =>
            {
                var rand = new Random(Environment.TickCount);
                while (true)
                {
                    string quote = (rand.NextDouble() * 100.0).ToString("0.00");
                    SendMessageAsync(socket, new Message { Data = quote, MessageType = MessageType.Text });
                    Thread.Sleep(2000);
                }
            });

            return base.OnConnected(socket);
        }

        public override Task OnDisconnected(WebSocket socket)
        {
            System.Diagnostics.Debug.WriteLine("Socket closed: " + WebSocketConnectionManager.GetId(socket));
            return base.OnDisconnected(socket);
        }
    }
}