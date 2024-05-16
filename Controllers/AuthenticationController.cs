using HRMS.Filters;
using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Globalization;
using System.Threading;
namespace HRMS.Controllers
{
    public class AuthenticationController : Controller
    {
        MayankEntities _db;

        public AuthenticationController()
        {
            _db = new MayankEntities();
        }
        // GET: Authorization/Login
        public ActionResult Login()
        {
            List<Task> _task = _db.Tasks.ToList();
            var _taskBygroup = _task.GroupBy(x => x.EmployeeId);
  
            foreach (var i in _taskBygroup)
            { 
                foreach(var j in i)
                {
                    Console.WriteLine("xckjdfnjb");
                }
                //int j = i.Count();
                //Console.WriteLine(j);
            }
            var cultureInfo = new CultureInfo("hi"); 
            Console.WriteLine(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            return View();
        }

        //  Post : Authorization/Login
        [HttpPost]
        public ActionResult Login(LoginUser _loginData)
        {
            Employee _emp = _db.Employees.Where(x => (x.Email == _loginData.Email && x.Password==_loginData.Password && x.isDeleted != true)).FirstOrDefault();
            if(_emp == null)
            {
                ViewBag.NoValidUser = "Invalid User Credentials";
                return View(_loginData);
            }
            else
            {

                Session["userName"] = _emp.FirstName + " " + _emp.LastName;
                Session["Role"] = _emp.Role.Name;
                Session["RoleId"] = _emp.DepartmentId;
                Session["userId"] = _emp.EmployeeId;
                Session["ReportingPersonId"] = _emp.ReportingPerson;

       
                if (_emp.DepartmentId == 1)
                {
                    TempData["LogInAsUser"] = "Logged In As A Director";
                    return RedirectToAction("AllEmployees", "Director");
                }
                else if(_emp.DepartmentId == 2)
                {
                    TempData["LogInAsUser"] = "Logged In As A Manager";
                    return RedirectToAction("GetAllTasks", "Director");
                }
                else
                {
                    TempData["LogInAsUser"] = "Logged In As A Employee";
                    return RedirectToAction("GetAllMyTask", "Director");
                }
               
            }
        }

        public ActionResult Logout()
        {
            Session["userName"] = null;
            Session["Role"] = null;
            Session["userId"] = null;
            Session["ReportingPersonId"] = null;
            return RedirectToAction("Login", "Authentication");
        }
        public ActionResult Error404()
        {
            return View();
        }
    }
}