using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebSocketDemo
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Task.Run(() =>
            {
                var rand = new Random(Environment.TickCount);
                while (true)
                {
                    string quote = (rand.NextDouble() * 100.0).ToString("#.##");
                    SocketHandler.SendMessage(quote);
                    Thread.Sleep(2000);
                }
            });
        }
    }
}
