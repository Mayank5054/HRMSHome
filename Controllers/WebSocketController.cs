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
using HRMS.Models;
using Microsoft.Ajax.Utilities;
namespace HRMS.Controllers
{
    [LoginFilter]
    public class WebSocketController : Controller
    {
        MayankEntities _db;
       public  WebSocketController()
        {
            _db = new MayankEntities();
        }
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
                    if (currentUsers.ContainsKey(jsonData["id"].ToString()) == true)
                    {
                        Chat _chatMessage = new Chat();
                        _chatMessage.sender = jsonData["senderId"];
                        _chatMessage.reciever = jsonData["id"];
                        _chatMessage.data = jsonData["message"];
                        _db.Chats.Add(_chatMessage);
                        _db.SaveChanges();
                        currentUsers[jsonData["id"].ToString()].Send(message);
                    }
                };
               
            });
        }
    }
}