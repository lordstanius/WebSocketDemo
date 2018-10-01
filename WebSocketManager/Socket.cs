using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public class Socket
    {
        private readonly WebSocketHandler _handler;
        private readonly WebSocket _webSocket;
        private readonly byte[] _buffer = new byte[1024 * 4];

        internal Socket(WebSocketHandler handler, WebSocket socket)
        {
            _handler = handler;
            _webSocket = socket;

            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; }

        public async Task SendMessageAsync(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
                return;

            var bytes = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(bytes);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task ReceiveAsync()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
                _handler.OnMessage(this, Encoding.UTF8.GetString(_buffer));
            else if (result.MessageType == WebSocketMessageType.Close)
                _handler.OnClosed(this);
        }
    }
}