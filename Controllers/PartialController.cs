using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Models;
namespace HRMS.Controllers
{
    public class PartialController : Controller
    {
        MayankEntities _db;
        public PartialController()
        {
            _db = new MayankEntities();
        }
        // GET: Partial
        public ActionResult AddTaskForm()
        {
            ViewBag.Operation = "Add";
            return PartialView("_TaskForm");
        }

        public ActionResult EditTaskForm(int id)
        {
            Task _task = _db.Tasks.Find(id);
            ViewBag.Operation = "Edit";
            return PartialView("_TaskForm",_task);
        }

        public ActionResult GetChat(int id)
        { 
            List<Chat> _listOfChats = _db.Chats.Where(x => x.reciever == id).ToList();
            return PartialView("_ChatHistory",_listOfChats);
        }
    }
}