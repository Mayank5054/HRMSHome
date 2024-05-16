using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Fleck;
using Newtonsoft.Json;
using HRMS.Filters;
namespace HRMS.Controllers
{
    [LoginFilter]
    public class WebSocketController : Controller
    {
        private static Dictionary<string, IWebSocketConnection> currentUsers = new Dictionary<string, IWebSocketConnection>();

        public ActionResult WebSocket()
        {
            return View();
        }

        public void StartWebSocket()
        {
          
            var server = new WebSocketServer("ws://0.0.0.0:5355");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    var userId = socket.ConnectionInfo.Path.Split('=')[1];
                    if (!currentUsers.ContainsKey(userId))
                    {
                        Console.Write("Connection Open");
                        currentUsers.Add(userId, socket);
                    }
                };
                socket.OnClose = () =>
                {
                    var userId = socket.ConnectionInfo.Path.Split('=')[1];
                  
                    currentUsers.Remove(userId);
                };
                socket.OnMessage = message =>
                {
                    dynamic jsonData = JsonConvert.DeserializeObject(message);
                    if (currentUsers[jsonData["id"].ToString()] != null)
                    {
                        currentUsers[jsonData["id"].ToString()].Send(message);
                    }
                };
               
            });
        }
    }
}