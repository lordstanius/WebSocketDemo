using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSocketManager
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socket = new WebSocketExt(_webSocketHandler, webSocket);

            await socket.StartReceiving();

            //TODO: Investigate the Kestrel exception thrown when this is the last middleware
            //await _next.Invoke(context);
        }
    }
}