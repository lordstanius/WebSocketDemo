using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public abstract class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, WebSocketExt> _sockets = new ConcurrentDictionary<string, WebSocketExt>();

        public virtual void OnOpened(WebSocketExt socket) { }
        public virtual void OnClosed(WebSocketExt socket) { }
        public virtual void OnMessage(WebSocketExt socket, string message) { }
        public virtual void OnMessage(WebSocketExt socket, ArraySegment<byte> bytes) { }

        internal void Add(WebSocketExt socket)
        {
            _sockets.TryAdd(socket.ID, socket);
            OnOpened(socket);
        }

        public void Close(WebSocketExt socket)
        {
            CloseAsync(socket).Wait();
        }

        public async Task CloseAsync(WebSocketExt socket)
        {
            _sockets.TryRemove(socket.ID, out WebSocketExt throwAwayValue);

            await socket.CloseAsync();
        }

        public async Task Broadcast(string message)
        {
            foreach (var entry in _sockets)
                await entry.Value.SendAsync(message);
        }
    }
}