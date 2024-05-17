using HRMS.Filters;
using HRMS.Models;
using HRMS.Security;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{

    [LoginFilter]
    public class DirectorController : Controller
    {
        MayankEntities _db;
        public DirectorController()
        {
            _db = new MayankEntities();
        }
        // GET: Director
        [@Authorize(new string[] { "Director" })]
        public ActionResult AddEmployee()
        {
           List<SelectListItem> role = _db.Roles.Select(
               x => new SelectListItem
               {
                   Text = x.Name,
                   Value = x.RoleId.ToString()
               }).ToList();

            List<SelectListItem> reportingPersons= _db.Employees.Where(x=> (x.DepartmentId == 2 || x.DepartmentId == 1 )).Select(
    x => new SelectListItem
    {
        Text = x.FirstName +" " +x.LastName,
        Value = x.EmployeeId.ToString()
    }).ToList();
            ViewBag.roles = role;
            ViewBag.reportingPersons = reportingPersons;
            return View();
        }

        [@Authorize(new string[] { "Director" })]
        [HttpPost]
        public ActionResult AddEmployee(Employee _emp)
        {
            Employee emp = _db.Employees.Where(x => x.Email == _emp.Email).FirstOrDefault();
            List<SelectListItem> role = _db.Roles.Select(
     x => new SelectListItem
     {
         Text = x.Name,
         Value = x.RoleId.ToString()
     }).ToList();

            List<SelectListItem> reportingPersons = _db.Employees.Where(x => (x.DepartmentId == 2 || x.DepartmentId == 1)).Select(
    x => new SelectListItem
    {
        Text = x.FirstName + " " + x.LastName,
        Value = x.EmployeeId.ToString()
    }).ToList();
            ViewBag.roles = role;
            ViewBag.reportingPersons = reportingPersons;
            if (emp != null)
            {
                //TempData["EmployeeExists"] = "User Already Exists";
                ViewBag.EmployeeExists = " User Already Exists";
                return View();

            }
            if (ModelState.IsValid)
            {
                string cipherText = EncryptionHelper.Encrypt(_emp.Password);
                Console.WriteLine(cipherText);
                string decryptedText = EncryptionHelper.Decrypt(cipherText);
                Console.WriteLine(decryptedText);

                int totalCount = _db.Employees.ToList().Count();
                _emp.EmployeeCode = "SIT-"+(totalCount+1);
                _emp.isDeleted = false;
                _db.Employees.Add(_emp);
                _db.SaveChanges(); 
                TempData["NewEmployeeAdded"] = "New Employee Added";
                return RedirectToAction("AllEmployees", "Director");
            }
            else
            {
                ViewBag.SomethingWrong = "Something Wrong Happen";
                //TempData["SomethingWrong"] = "Something Wrong Happen";
                return View();
            }
           
        }

        [@Authorize(new string[] { "Director" })]
        public ActionResult EditEmployee(int id)
        {
            Employee _emp = _db.Employees.Find(id);
            List<SelectListItem> role = _db.Roles.Select(
              x => new SelectListItem
              {
                  Text = x.Name,
                  Value = x.RoleId.ToString()
              }).ToList();

            List<SelectListItem> reportingPersons = _db.Employees.Where(x => (x.DepartmentId == 2 || x.DepartmentId == 1)).Select(
    x => new SelectListItem
    {
        Text = x.FirstName + " " + x.LastName,
        Value = x.EmployeeId.ToString()
    }).ToList();
            ViewBag.roles = role;
            ViewBag.reportingPersons = reportingPersons;
            return View(_emp) ;
        }

        [@Authorize(new string[] { "Director" })]
        [HttpPost]
        public ActionResult EditEmployee(Employee _emp)
        {
            if (ModelState.IsValid)
            {
                Employee emp = _db.Employees.Find(_emp.EmployeeId);
                if (emp != null  && (_emp.DepartmentId <= emp.DepartmentId))
                {
                    emp.Email = _emp.Email;
                    emp.Password = _emp.Password;
                    emp.FirstName = _emp.FirstName;
                    emp.LastName = _emp.LastName;
                    emp.BirthDate = _emp.BirthDate;
                    emp.Gender = _emp.Gender;
                    emp.DepartmentId = _emp.DepartmentId;
                    emp.ReportingPerson = _emp.ReportingPerson;
                    _db.SaveChanges();
                    TempData["EmployeeEdited"] = " Emeployee Details Edited";
                    return RedirectToAction("AllEmployees");
                }
                else
                {
                    TempData["EmployeeMissing"] = "Can not Demote The Employee";
                  
                    return RedirectToAction("EditEmployee");
                }
                
            }
            else
            {
                return View();
            }
            
        }

        [@Authorize(new string[] { "Director" })]
        public ActionResult DeleteEmployee(int id)
        {

            Employee emp = _db.Employees.Find(id);
           
            if (emp != null)
            {

                if (emp.DepartmentId == 2)
                {
                    emp.isDeleted = true;
                    _db.SaveChanges();
                    return Json(new { status = "Success", message = "Manager deleted" }, JsonRequestBehavior.AllowGet);

                }
                else if(emp.DepartmentId == 1)
                {
                    return Json(new { status = "Failed", message = "Trying to delete Director" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    emp.isDeleted = true;
                    //_db.Employees.Remove(emp);
                    _db.SaveChanges();
                    return Json(new { status = "Success",message = "Employee Deleted" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "Failed", message = "Employee Not Found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [@Authorize(new string[] { "Director" })]
        public ActionResult AllEmployees()
        {

        
            List<Employee> _emp = _db.Employees.Include("Employee2")
                .Where(x => x.isDeleted != true)
                .ToList();
            return View(_emp);
        }
        //Convert.ToInt32(Session["RoleId"].ToString())
        [@Authorize(new string[] {"Director","Manager"})]
        public ActionResult GetAllTasks()
        {
            int roleId = int.Parse(Session["RoleId"].ToString());
           if(roleId != 2)
            {
                List<HRMS.Models.Task> _taskData = _db.Tasks
                            .Include("Employee2")
                            .Include("Employee")
                           .Where(x => x.Employee2.DepartmentId > roleId)
                           .Take(5)
                           .ToList();
                return View(_taskData);
            }
            else
            {
                int userId = int.Parse(Session["userId"].ToString());
                List<HRMS.Models.Task> _taskData = _db.Tasks
                          .Where(x => x.ApproverID == userId)
                          .Take(2)
                          .ToList();
                return View(_taskData);
            }
        }

        [@Authorize(new string[] { "Director","Manager" })]
        public ActionResult ApproveTask(int id,string status)
        {
            Task _task = _db.Tasks.Find(id);
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            DateTime currentDate = DateTime.Now;
            string customShortDateString = currentDate.ToString("yyyy-MM-dd");
           if(_task.ApprovedORRejectedBy != null && _task.ApprovedORRejectedBy == 1)
            {
                    TempData["TaskApprovedByDirector"] = "Tasks Approved By Director Can Not Be Altered " ;
                    return RedirectToAction("GetAllTasks");
            }
            else
            {
                if (_task != null && (_task.ApproverID == userId || roleId == 1))
                {
                    _task.ApprovedORRejectedBy = userId;
                    _task.Status = status;
                    _task.ApprovedORRejectedOn = currentDate;
                    _task.ModifiedOn = currentDate;
                    _db.SaveChanges();
                    TempData["TaskStatus"] = "Task Has Been " + status;
                    return RedirectToAction("GetAllTasks");
                }
                TempData["UnAuthorized"] = "UnAuthorized ";
                return RedirectToAction("GetAllTasks");
            }
        }
        [@Authorize(new string[] { "Director", "Manager" })]
        public ActionResult RejectTask(int id)
        {
            Task _task = _db.Tasks.Find(id);
            int userId = int.Parse(Session["userId"].ToString());
            DateTime currentDate = DateTime.Now;
            string customShortDateString = currentDate.ToString("yyyy-MM-dd");
            if (_task != null)
            {
                _task.ApprovedORRejectedBy = userId;
                _task.Status = "Rejected";
                _task.ApprovedORRejectedOn = currentDate;
                _task.ModifiedOn = currentDate;
                _db.SaveChanges();
            }
            return RedirectToAction("GetAllTasks");
        }

        [@Authorize(new string[] {  "Manager","Employee" })]
        public ActionResult AddTask()
        {   
            return View();
        }

        [@Authorize(new string[] { "Manager", "Employee" })]
        [HttpPost]
        public ActionResult AddTask(Task _task)
        {
            if (ModelState.IsValid)
            {
                DateTime currentDate = DateTime.Now;
                string customShortDateString = currentDate.ToString("yyyy-MM-dd");
                _task.ApproverID = (int)Session["ReportingPersonId"];
                _task.TaskDate = currentDate;
                _task.EmployeeId = (int)Session["userId"];
                _task.ApprovedORRejectedBy = null;
                _task.ApprovedORRejectedOn = null;
                _task.Status = "Pending";
                _task.CreatedOn= currentDate;
                _task.ModifiedOn= currentDate;
                _db.Tasks.Add( _task );
                _db.SaveChanges();
                TempData["TaskAdded"] = "Task Has Been Added";
                return RedirectToAction("GetAllmYTask");
            }
            return View();
        }
        [@Authorize(new string[] { "Manager", "Employee" })]
        public  ActionResult GetAllMyTask()
        {
            int userId = int.Parse(Session["userId"].ToString());
            List<Task> _taskData = _db.Tasks.Include("Employee").Where(x=>x.EmployeeId == userId).Take(5).ToList();
            return View(_taskData);
        }

        [@Authorize(new string[] { "Manager", "Employee" })]
        public ActionResult EditTask(int id)
        {
            int userId = int.Parse(Session["userId"].ToString());
            Task _task = _db.Tasks.Find(id);
            if (_task != null && userId != null)
            {
                if(_task.EmployeeId == userId)
                {
                    return View(_task);
                }
                else
                {
                    return Redirect("~/Authentication/Error404");
                }
            }
            else
            {
                TempData["TaskNotFound"] = "Task Not Found In The Record";
                return RedirectToAction("GetAllMyTask");
            }
            
        }

        [@Authorize(new string[] { "Manager", "Employee" })]
        [HttpPost]
        public ActionResult EditTask(Task task)
        {
            Task _task = _db.Tasks.Find(task.TaskId);
            if (_task != null)
            {
                _task.TaskDate = task.TaskDate;
                _task.TaskName = task.TaskName;
                _task.TaskDescription = task.TaskDescription;
                _db.SaveChanges();
                TempData["TaskUpdated"] = "Task Updated Successfully";
                return RedirectToAction("GetAllMyTask");
            }
            return View(_task);
        }
        [@Authorize(new string[] { "Manager", "Employee" })]
        public ActionResult DeleteTask(int id)
        {
            int userId = int.Parse(Session["userId"].ToString());
            Task _task = _db.Tasks.Find(id);
            if (_task != null)
            {
                if (_task.EmployeeId == userId)
                {
                    _db.Tasks.Remove(_task);
                    _db.SaveChanges();
                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Redirect("~/Authentication/Error404");
                }
            }
            TempData["TaskNotFound"] = "Task Not Found In The Record";
            return RedirectToAction("GetAllMyTask");
           
        }

        public ActionResult GetProfile()
        {
            int userId = int.Parse(Session["userId"].ToString());
            Employee _profile = _db.Employees.Find(userId);
            return View(_profile);
        }

        public ActionResult EditProfile(int id)
        {
          
            List<SelectListItem> role = _db.Roles.Select(
              x => new SelectListItem
              {
                  Text = x.Name,
                  Value = x.RoleId.ToString()
              }).ToList();

            List<SelectListItem> reportingPersons = _db.Employees.Where(x => (x.DepartmentId == 2 || x.DepartmentId == 1)).Select(
    x => new SelectListItem
    {
        Text = x.FirstName + " " + x.LastName,
        Value = x.EmployeeId.ToString()
    }).ToList();
            ViewBag.roles = role;
            ViewBag.reportingPersons = reportingPersons;
            int userId = int.Parse(Session["userId"].ToString());
            if (userId == id)
            {
                Employee _emp = _db.Employees.Find(id);
                
                    return View(_emp);
            }
            else
            {
                return Redirect("~/Authentication/Error404");
            }

        }
        [HttpPost]
        public ActionResult EditProfile(Employee _emp)
        {
            List<SelectListItem> role = _db.Roles.Select(
              x => new SelectListItem
              {
                  Text = x.Name,
                  Value = x.RoleId.ToString()
              }).ToList();

            List<SelectListItem> reportingPersons = _db.Employees.Where(x => (x.DepartmentId == 2 || x.DepartmentId == 1)).Select(
    x => new SelectListItem
    {
        Text = x.FirstName + " " + x.LastName,
        Value = x.EmployeeId.ToString()
    }).ToList();
            ViewBag.roles = role;
            ViewBag.reportingPersons = reportingPersons;
           
                Employee emp = _db.Employees.Find(_emp.EmployeeId);
                if (emp != null)
                {
                    emp.Email = _emp.Email;
                    emp.Password = _emp.Password;
                    emp.FirstName = _emp.FirstName;
                    emp.LastName = _emp.LastName;
                    emp.BirthDate = _emp.BirthDate;
                emp.Gender= _emp.Gender;
                    _db.SaveChanges();
                TempData["ProfileUpdated"] = "Profile Updated Successfully";
                    return RedirectToAction("GetProfile");
                }
                return View();
           
            
        }

        public ActionResult FiredEmployees()
        {
            List<Employee> firedEmp = _db.Employees.Include("TeamsDetails")
                //.Include("TeamsDetails.Team.TeamLeader.Employee")
               .Where(x => x.isDeleted == true).ToList();
            return View(firedEmp);
        }

        public ActionResult HireAgain(int id)
        {
            Employee employee = _db.Employees.Find(id);
            if (employee != null && employee.isDeleted == true)
            {
                employee.isDeleted = false;
                _db.SaveChanges();
                return Json(new { status = "Success", message = "Employee Has Been ReHired" },JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { status = "Failure", message = "Employee Not found or Already Hired" },JsonRequestBehavior.AllowGet);
            }
           
        }
        public ActionResult GetReportingPersons(int id)
        {
            int userId = int.Parse(Session["userId"].ToString());
            Employee emp = _db.Employees.Find(userId);
            string jsonData;
           
            
                List<SelectListItem> reportingPersons = _db.Employees.Where(x => x.DepartmentId < id).Select(
x => new SelectListItem
{
   Text = x.FirstName + " " + x.LastName,
   Value = x.EmployeeId.ToString()
}).ToList();

                jsonData = JsonConvert.SerializeObject(reportingPersons);
         
            return Json(new { reportingPersons = jsonData }, JsonRequestBehavior.AllowGet);
        }


       


    }
}