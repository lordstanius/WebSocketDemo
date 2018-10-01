using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public abstract class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, Socket> _sockets = new ConcurrentDictionary<string, Socket>();

        public abstract void OnOpened(Socket socket);
        public abstract void OnClosed(Socket socket);
        public abstract void OnMessage(Socket socket, string message);

        public void Add(Socket socket)
        {
            _sockets.TryAdd(socket.ID, socket);
            
            OnOpened(socket);
        }

        public async Task Broadcast(string message)
        {
            foreach (var entry in _sockets)
                await entry.Value.SendMessageAsync(message);
        }
    }
}