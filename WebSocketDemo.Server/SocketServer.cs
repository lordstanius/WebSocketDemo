using Microsoft.Web.WebSockets;

namespace WebSocketDemo
{
    public class SocketServer : WebSocketHandler
    {
        private static readonly WebSocketCollection Sockets = new WebSocketCollection();
        private readonly string _sessionId;

        public SocketServer(string sessionId)
        {
            _sessionId = sessionId;
        }

        public override void OnOpen()
        {
            Sockets.Add(this);
            base.OnOpen();
        }

        public override void OnClose()
        {
            Sockets.Remove(this);
            base.OnClose();
        }

        public static void SendMessage(string message)
        {
            Sockets.Broadcast(message);
        }
    }
}