using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSocketServer.Middleware
{
    public class WebSocketServerMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
        {
            _manager = manager;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
             if(context.WebSockets.IsWebSocketRequest)
               {
                   WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                   System.Console.WriteLine("--> WebSocket Connected");

                    string ConnId = _manager.AddSocket(webSocket);
                    await SendConnIdAsync(webSocket, ConnId);

                    await ReceiveMessage(webSocket, async(result, buffer) => 
                    {
                        if(result.MessageType == WebSocketMessageType.Text)
                        {
                            System.Console.WriteLine("--> Message Received");
                            System.Console.WriteLine($"--> Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                            return;
                        } 
                        else if(result.MessageType == WebSocketMessageType.Close)
                        {
                            System.Console.WriteLine("--> Recived Close message");
                            return;
                        }
                    });
               } else {
                    System.Console.WriteLine("--> Helo from the 2nd request delegate");
                    await _next(context);
               }
        }

        private async Task SendConnIdAsync(WebSocket socket, string connId)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnId: {connId}");
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while(socket.State == WebSocketState.Open){
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: System.Threading.CancellationToken.None);

                    handleMessage(result, buffer);
            }
        }
    }
}