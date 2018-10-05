using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSocketManager
{
    public class WebSocketManagerMiddleware
    {
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate _, WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socket = new WebSocketConnection(_webSocketHandler, webSocket);

            await socket.StartReceiving();
        }
    }
}