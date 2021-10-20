using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;

namespace WebSocketServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region WebSocker
           app.UseWebSockets();

           app.Use(async (context, next) => 
           {
               WriteRequestParam(context);
               if(context.WebSockets.IsWebSocketRequest)
               {
                   WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                   System.Console.WriteLine("--> WebSocket Connected");
               } else {
                    System.Console.WriteLine("--> Helo from the 2nd request delegate");
                    await next();
               }
           });

           app.Run(async context => 
           {
               System.Console.WriteLine("--> Helo from the 3rd request delegate");
               await context.Response.WriteAsync("--> Helo from the 3rd request delegate");
           });
           #endregion WebSocket
        }

        public void WriteRequestParam(HttpContext context)
        {
            System.Console.WriteLine($"--> Request Method: {context.Request.Method}");
            System.Console.WriteLine($"--> Request Protocol: {context.Request.Method}");

            if(context.Request.Headers != null)
            {
                foreach(var h in context.Request.Headers)
                {
                    System.Console.WriteLine($"--> {h.Key} : {h.Value}");
                }
            }
        }
    }
}
