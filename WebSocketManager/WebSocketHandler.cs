using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public abstract class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, WebSocketConnection> _sockets = new ConcurrentDictionary<string, WebSocketConnection>();

        public virtual void OnOpened(WebSocketConnection socket) { }
        public virtual void OnClosed(WebSocketConnection socket) { }
        public virtual void OnMessage(WebSocketConnection socket, string message) { }
        public virtual void OnMessage(WebSocketConnection socket, ArraySegment<byte> bytes) { }

        internal void Add(WebSocketConnection socket)
        {
            _sockets.TryAdd(socket.ID, socket);
            OnOpened(socket);
        }

        public void Close(WebSocketConnection socket)
        {
            CloseAsync(socket).Wait();
        }

        public async Task CloseAsync(WebSocketConnection socket)
        {
            _sockets.TryRemove(socket.ID, out WebSocketConnection throwAwayValue);

            await socket.CloseAsync();
        }

        public async Task Broadcast(string message)
        {
            foreach (var entry in _sockets)
                await entry.Value.SendAsync(message);
        }
    }
}