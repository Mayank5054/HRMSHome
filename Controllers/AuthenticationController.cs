using HRMS.Filters;
using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View();
        }

        //  Post : Authorization/Login
        [HttpPost]
        public ActionResult Login(LoginUser _loginData)
        {
            Employee _emp = _db.Employees.Where(x => x.Email == _loginData.Email && x.Password==_loginData.Password).FirstOrDefault();
            if(_emp == null)
            {
                return View(_loginData);
            }
            else
            {
                Session["userName"] = _emp.FirstName + _emp.LastName;
                Session["Role"] = _emp.Role.Name;
                Session["RoleId"] = _emp.DepartmentId;
                Session["userId"] = _emp.EmployeeId;
                Session["ReportingPersonId"] = _emp.ReportingPerson;
                if(_emp.DepartmentId == 1)
                {
                    return RedirectToAction("AllEmployees", "Director");
                }
                else if(_emp.DepartmentId == 2)
                {
                    return RedirectToAction("GetAllTasks", "Director");
                }
                else
                {
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