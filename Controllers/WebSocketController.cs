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
        public static  void NotifyOffine(int id)
        {
            foreach (var item in currentUsers)
            {
                item.Value.Send(JsonConvert.SerializeObject(new { operation = "userGetsOffline", userId = id }));
            }
          
        }
        public void closeConnection(int id)
        {
            //var userId = socket.ConnectionInfo.Path.Split('=')[1];
            Employee _emp = _db.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            _emp.isOnline = false;
            _db.SaveChanges();
            foreach (var item in currentUsers)
            {
                item.Value.Send(JsonConvert.SerializeObject(new { operation = "userGetsOffline", userId = id }));
            }
            currentUsers[id.ToString()].Close();
            currentUsers.Remove(id.ToString());
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
                        Employee _emp = _db.Employees.Where(x => x.EmployeeId.ToString() == userId).FirstOrDefault();
                        _emp.isOnline = true;
                        _db.SaveChanges();
                        foreach (var item in currentUsers)
                        {
                        item.Value.Send(JsonConvert.SerializeObject(new{operation="userGetsOnline",userId=userId }));
                        }
                        Console.Write("Connection Open");
                        currentUsers.Add(userId, socket);
                    }
                };
                socket.OnClose = () =>
                {
                    var userId = socket.ConnectionInfo.Path.Split('=')[1];
                    Employee _emp = _db.Employees.Where(x => x.EmployeeId.ToString() == userId).FirstOrDefault();
                    _emp.isOnline = false;
                    _db.SaveChanges();
                    foreach (var item in currentUsers)
                    {
                        item.Value.Send(JsonConvert.SerializeObject(new { operation = "userGetsOffline", userId = userId }));
                    }
                    currentUsers[userId.ToString()].Close();
                    currentUsers.Remove(userId);
              
                };
                socket.OnMessage = message =>
                {
                    dynamic jsonData = JsonConvert.DeserializeObject(message);
                    if(jsonData.status == "MARKSEEN")
                    {

                    }
                    if (currentUsers.ContainsKey(jsonData["id"].ToString()) == true)
                    {
                        Chat _chatMessage = new Chat();
                        _chatMessage.sender = jsonData["senderId"];
                        _chatMessage.reciever = jsonData["id"];
                        _chatMessage.data = jsonData["message"];
                        _chatMessage.isQueued = 0;
                        _chatMessage.delivered = DateTime.UtcNow;
                        _chatMessage.seen = null;
                        _db.Chats.Add(_chatMessage);
                        _db.SaveChanges();
                        currentUsers[jsonData["id"].ToString()].Send(message);
                    }
                    else
                    {
                        Chat _chatMessage = new Chat();
                        _chatMessage.sender = jsonData["senderId"];
                        _chatMessage.reciever = jsonData["id"];
                        _chatMessage.data = jsonData["message"];
                        _chatMessage.isQueued = 1;
                        _chatMessage.delivered = null;
                        _chatMessage.seen = null;
                        _db.Chats.Add(_chatMessage);
                        _db.SaveChanges();
                 
                    }
                };
            });
        }
    }
}