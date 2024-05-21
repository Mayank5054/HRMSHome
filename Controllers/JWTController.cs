using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class JWTController : Controller
    {
        // GET: JWT
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginUser _loginUser)
        {
            Console.WriteLine(_loginUser);
            var json = 2;
            return Json(new { json });
        }
    }
}