using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class PartialController : Controller
    {
        // GET: Partial
        public ActionResult AddTaskForm()
        {
            return PartialView("_AddTaskForm");
        }
    }
}