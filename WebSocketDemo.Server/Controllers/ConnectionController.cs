﻿using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Web.WebSockets;

namespace WebSocketDemo.Controllers
{
    public class ConnectionController : ApiController
    {
        public HttpResponseMessage Get(string id = null)
        {
            var context = HttpContext.Current;
            if (context.IsWebSocketRequest || context.IsWebSocketRequestUpgrading)
            {
                context.AcceptWebSocketRequest(new SocketHandler(id));
                return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}