using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class FunController : Controller
    {
        // GET: Fun
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(List<fun> _obj)
        {
            foreach(fun i in _obj)
            {
                
            }
            return View();
        }
    }
}