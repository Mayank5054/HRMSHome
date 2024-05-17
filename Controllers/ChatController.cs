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
            List<Chat> _listOfChats = _db.Chats.Where(x=> (x.reciever == userId || x.sender == userId)).ToList();
            return View(_listOfChats);

        }
    }
}