using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public class WebSocketExt
    {
        private readonly WebSocket _socket;
        private readonly WebSocketHandler _handler;
        private readonly byte[] _buffer = new byte[1024];

        internal WebSocketExt(WebSocketHandler handler, WebSocket socket)
        {
            ID = Guid.NewGuid().ToString();
            _socket = socket;
            _handler = handler;
            _handler.Add(this);
        }

        public string ID { get; }

        public async Task StartReceiving()
        {
            while (_socket.State == WebSocketState.Open)
                await ReceiveAsync();
        }

        private async Task ReceiveAsync()
        {
            var buffer = new ArraySegment<byte>(_buffer);
            var result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.EndOfMessage)
            {
                PassMessageToHandler(result, buffer);
                return;
            }

            using (var stream = new MemoryStream(buffer.Array.Length * 2))
            {
                stream.Write(buffer.Array, 0, result.Count);
                while (!result.EndOfMessage)
                {
                    result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    stream.Write(buffer.Array, 0, result.Count);
                }

                if (stream.TryGetBuffer(out ArraySegment<byte> bytes))
                    PassMessageToHandler(result, bytes);
            }
        }

        private void PassMessageToHandler(WebSocketReceiveResult result, ArraySegment<byte> buffer)
        {
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    _handler.OnMessage(this, message);
                    break;
                case WebSocketMessageType.Binary:
                    _handler.OnMessage(this, buffer);
                    break;
                case WebSocketMessageType.Close:
                    _socket.Dispose();
                    _handler.OnClosed(this);
                    break;
            }
        }

        public async Task CloseAsync()
        {
            await _socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closed by WebSocketManager",
                CancellationToken.None);

            _socket.Dispose();
            _handler.OnClosed(this);
        }

        public async Task SendAsync(string message)
        {
            await SendAsyncInternal(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text);
        }

        public async Task SendAsync(byte[] bytes)
        {
            await SendAsyncInternal(bytes, WebSocketMessageType.Binary);
        }

        private async Task SendAsyncInternal(byte[] bytes, WebSocketMessageType messageType)
        {
            if (_socket.State != WebSocketState.Open)
                return;

            var buffer = new ArraySegment<byte>(bytes);
            await _socket.SendAsync(buffer, messageType, true, CancellationToken.None);
        }
    }
}