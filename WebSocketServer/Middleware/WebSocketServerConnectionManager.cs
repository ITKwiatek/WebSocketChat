using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
namespace WebSocketServer.Middleware
{
    public class WebSocketServerConnectionManager
    {
        //singleton
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }

        public string AddSocket(WebSocket socket)
        {
            string ConnId = Guid.NewGuid().ToString();
            _sockets.TryAdd(ConnId, socket);
            System.Console.WriteLine($"--> Connection Added to List {ConnId}");

            return ConnId;
        }
    }
}