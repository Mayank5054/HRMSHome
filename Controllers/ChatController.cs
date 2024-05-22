using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Filters;
namespace HRMS.Controllers
{
    [LoginFilter]
    public class ChatController : Controller
    {
        MayankEntities _db;
        // GET: Chat
        public ChatController() { 
        _db = new MayankEntities();
        }

        [@Authorize(new string[] {"Employee","Manager"})]
        public ActionResult Index()
        {
            int userId = (int)Session["userId"];
            ChatExtended _ce = new ChatExtended
            {
                listOfSeenChats = _db.Chats.Where(x => ((x.reciever == userId || x.sender == userId) && (x.isQueued == null))).ToList(),
                listOfUsSeenChats = _db.Chats.Where(x => ((x.reciever == userId || x.sender == userId) && (x.isQueued == 1))).ToList()
            };

   
                foreach(Chat i in _ce.listOfUsSeenChats)
            {
                i.isQueued = 0;
                i.delivered = DateTime.UtcNow;
                i.seen = DateTime.UtcNow;
            }
                _db.SaveChanges();
            return View(_ce);

        }
    }
}