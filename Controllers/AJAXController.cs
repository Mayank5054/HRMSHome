using HRMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AJAXController : Controller
    {
        MayankEntities _db;
        public AJAXController()
        {
            _db = new MayankEntities();
        }

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
                _task.CreatedOn = currentDate;
                _task.ModifiedOn = currentDate;
                _db.Tasks.Add(_task);
                _db.SaveChanges();
                var taskData = _db.Tasks.Include("Employee1")
                    .Include("Employee2")
                    .Include("Employee")
                    .Where(x => x.TaskId == _task.TaskId)
                      .Select(x => new
                      {
                          ApproverFirstName = x.Employee1.FirstName,
                          ApproverLastName = x.Employee1.LastName,
                           ApprovedByFirstName = x.Employee.FirstName,
                          ApprovedByLastName = x.Employee.LastName,
                          ApprovedDate = x.ApprovedORRejectedOn,
                          x.TaskId,
                          x.TaskDate,
                          x.TaskName,
                          x.TaskDescription,
                          x.Status, x.ModifiedOn,
                    
                      })
                    .FirstOrDefault();
                string jsonData = JsonConvert.SerializeObject(taskData);
                return Json(new { status = "Success", message = "Task Has Been Added",data=jsonData });
               
            }
            return Json(new { status = "Failure", message = "Task Creation Failed" });
        }

       
            public ActionResult EditTask(Task task)
            {
                Task _task = _db.Tasks.Find(task.TaskId);
                if (_task != null)
                {
                    _task.TaskDate = task.TaskDate;
                    _task.TaskName = task.TaskName;
                    _task.TaskDescription = task.TaskDescription;
                    _db.SaveChanges();

                var taskData = _db.Tasks.Include("Employee1")
                   .Include("Employee2")
                   .Include("Employee")
                   .Where(x => x.TaskId == _task.TaskId)
                     .Select(x => new
                     {
                         ApproverFirstName = x.Employee1.FirstName,
                         ApproverLastName = x.Employee1.LastName,
                         ApprovedByFirstName = x.Employee.FirstName,
                         ApprovedByLastName = x.Employee.LastName,
                         ApprovedDate = x.ApprovedORRejectedOn,
                         x.TaskId,
                         x.TaskDate,
                         x.TaskName,
                         x.TaskDescription,
                         x.Status,
                         x.ModifiedOn,

                     })
                   .FirstOrDefault();

                string jsonData = JsonConvert.SerializeObject(taskData);
                return Json(new { status = "Success", message = "Task Has Been Updated", data = jsonData });

            }
            else
            {
                return Json(new { status = "Failure", message = "Task Updation Failed" });
            }
                ;
            }
   
        public ActionResult FetchFromTo(int from,int to)
        {
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            int min = _db.Employees.Min(x=>x.EmployeeId);
            object taskData = new {};
            if (to == -1)
            {
               taskData = _db.Tasks.Include("Employee1")
                       .Include("Employee2")
                       .Include("Employee")
                        .Where(x => x.EmployeeId == userId)
                       .OrderBy(x => x.TaskDate)
                       .Skip(from)
                       .Select(x => new
                       {
                           ApproverFirstName = x.Employee1.FirstName,
                           ApproverLastName = x.Employee1.LastName,
                           ApprovedByFirstName = x.Employee.FirstName,
                           ApprovedByLastName = x.Employee.LastName,
                           ApprovedDate = x.ApprovedORRejectedOn,
                           x.TaskId,
                           x.TaskDate,
                           x.TaskName,
                           x.TaskDescription,
                           x.Status,
                           x.ModifiedOn,

                       })
                       .ToList();
            }
            else
            {
                 taskData = _db.Tasks.Include("Employee1")
                    .Include("Employee2")
                    .Include("Employee")
                     .Where(x => x.EmployeeId == userId)
                    .OrderBy(x => x.TaskDate)
                    .Skip(from)
                    .Take(to-from)
                    .Select(x => new
                    {
                        ApproverFirstName = x.Employee1.FirstName,
                        ApproverLastName = x.Employee1.LastName,
                        ApprovedByFirstName = x.Employee.FirstName,
                        ApprovedByLastName = x.Employee.LastName,
                        ApprovedDate = x.ApprovedORRejectedOn,
                        x.TaskId,
                        x.TaskDate,
                        x.TaskName,
                        x.TaskDescription,
                        x.Status,
                        x.ModifiedOn,

                    })
                    .ToList();
            }
         

            string jsonData = JsonConvert.SerializeObject(taskData);
            return Json(new { status = "Success", message = "Task Has Been Sended", data = jsonData },JsonRequestBehavior.AllowGet);
        
        }
        public ActionResult FetchTasksFromTo(int from, int to)
        {
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            object taskData = new { };

            if (to == -1)
            {
                if (roleId != 2)
                {
                    taskData = _db.Tasks
                                .Include("Employee2")
                                .Include("Employee")
                               .Where(x => x.Employee2.DepartmentId > roleId)
                               .OrderBy(x => x.TaskDate)
                               .Skip(from)
                               .Select(x => new
                               {
                                   EmployeeName = x.Employee2.FirstName + " " + x.Employee2.LastName,
                                   ApproverFirstName = x.Employee1.FirstName,
                                   ApproverLastName = x.Employee1.LastName,
                                   ApprovedByFirstName = x.Employee.FirstName,
                                   ApprovedByLastName = x.Employee.LastName,
                                   ApprovedDate = x.ApprovedORRejectedOn,
                                   x.TaskId,
                                   x.TaskDate,
                                   x.TaskName,
                                   x.TaskDescription,
                                   x.Status,
                                   x.ModifiedOn,
                                   ApproverRole = x.Employee.Role.Name,
                               }).ToList();
                }
                else
                {
                    taskData = _db.Tasks.Include("Employee1")
              .Include("Employee2")
              .Include("Employee")
              .Where(x => x.ApproverID == userId)
              .OrderBy(x => x.TaskDate)
              .Skip(from)
              .Select(x => new
              {
                  EmployeeName = x.Employee2.FirstName + " " + x.Employee2.LastName,
                  ApproverFirstName = x.Employee1.FirstName,
                  ApproverLastName = x.Employee1.LastName,
                  ApprovedByFirstName = x.Employee.FirstName,
                  ApprovedByLastName = x.Employee.LastName,
                  ApprovedDate = x.ApprovedORRejectedOn,
                  x.TaskId,
                  x.TaskDate,
                  x.TaskName,
                  x.TaskDescription,
                  x.Status,
                  x.ModifiedOn,
                  ApproverRole = x.Employee.Role.Name,
              })
              .ToList();

                }
            }
            else
            {
                if (roleId != 2)
                {
                    taskData = _db.Tasks
                                .Include("Employee2")
                                .Include("Employee")
                               .Where(x => x.Employee2.DepartmentId > roleId)
                               .OrderBy(x => x.TaskDate)
                               .Skip(from)
                               .Take(to-from)
                               .Select(x => new
                               {
                                   EmployeeName = x.Employee2.FirstName + " " + x.Employee2.LastName,
                                   ApproverFirstName = x.Employee1.FirstName,
                                   ApproverLastName = x.Employee1.LastName,
                                   ApprovedByFirstName = x.Employee.FirstName,
                                   ApprovedByLastName = x.Employee.LastName,
                                   ApprovedDate = x.ApprovedORRejectedOn,
                                   x.TaskId,
                                   x.TaskDate,
                                   x.TaskName,
                                   x.TaskDescription,
                                   x.Status,
                                   x.ModifiedOn,
                                   ApproverRole = x.Employee.Role.Name,
                               }).ToList();
                }
                else
                {
                    taskData = _db.Tasks.Include("Employee1")
              .Include("Employee2")
              .Include("Employee")
              .Where(x => x.ApproverID == userId)
              .OrderBy(x => x.TaskDate)
              .Skip(from)
              .Take(to - from)
              .Select(x => new
              {
                  EmployeeName = x.Employee2.FirstName + " " + x.Employee2.LastName,
                  ApproverFirstName = x.Employee1.FirstName,
                  ApproverLastName = x.Employee1.LastName,
                  ApprovedByFirstName = x.Employee.FirstName,
                  ApprovedByLastName = x.Employee.LastName,
                  ApprovedDate = x.ApprovedORRejectedOn,
                  x.TaskId,
                  x.TaskDate,
                  x.TaskName,
                  x.TaskDescription,
                  x.Status,
                  x.ModifiedOn,
                  ApproverRole = x.Employee.Role.Name,
              })
              .ToList();

                }
                

            }
            string jsonData = JsonConvert.SerializeObject(taskData);
            return Json(new { status = "Success", message = "Task Has Been Sended", data = jsonData }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApproveTask(int id, string status)
        {
            Task _task = _db.Tasks.Find(id);
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            DateTime currentDate = DateTime.Now;
            string customShortDateString = currentDate.ToString("yyyy-MM-dd");
            if (_task.ApprovedORRejectedBy != null && _task.ApprovedORRejectedBy == 1)
            {
                return Json(new { status = "Failure", message = "Tasks Approved By Director Can Not Be Altered" }, JsonRequestBehavior.AllowGet);
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
                    Employee _emp = _db.Employees.Find(userId);
                    var data = new
                    {
                        ApprovedOrRejectedBy = _emp.FirstName + " " + _emp.LastName,
                        ApprovedORRejectedOn = currentDate
                    };
                    TempData["TaskStatus"] = "Task Has Been " + status;
                    return Json(new { status = "Success", message = "Task Has Been " + status,data1 =data}, JsonRequestBehavior.AllowGet);
                }
                TempData["UnAuthorized"] = "UnAuthorized ";
                return Json(new { status = "Failure", message = "UnAuthorized Access" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult HandleDataTable()
       {
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            int draw = Convert.ToInt32(Request["draw"]);
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            var sortColumn = Request["columns[" + Request["order[0][column]"] + "][name]"];
            var sortColumnDirection = Request["order[0][dir]"];

            var data = _db.Tasks.Where(x => x.EmployeeId == userId);

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>  
                (x.Employee1.FirstName + " " + x.Employee1.LastName).Contains(searchValue) &&
                (x.Employee.FirstName + " " + x.Employee.LastName).Contains(searchValue)
                );
            }
            if(!string.IsNullOrEmpty(sortColumn)) {
             
                switch (sortColumn)
                {
                    case "TaskDate":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.TaskDate);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.TaskDate);
                        }
                        break;
                  
                    case "TaskName":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.TaskName);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.TaskName);
                        }
                        break;
                    case "Status":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.Status);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.Status);
                        }
                        break;
                    case "Approver":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.Employee.FirstName + " " + x.Employee.LastName);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.Employee.FirstName + " " + x.Employee.LastName);
                        }
                        break;
                    case "ApprovedOn":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.ApprovedORRejectedOn);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.ApprovedORRejectedOn);
                        }
                        break;
                    default:
                        data = data.OrderByDescending(x => x.TaskDate);
                        break;
                }
            }
            else
            {
                data = data.OrderBy(x => x.TaskDate);
            }
           var data1 = data
                .Skip(start)
                .Take(length)
                .Select(x => new {
                ApprovedDate = x.ApprovedORRejectedOn,
                x.TaskId,
                x.TaskDate,
                x.TaskName,
                x.TaskDescription,
                x.Status,
                x.ModifiedOn,
                ApprovedOn = x.ApprovedORRejectedOn,
                Approver = x.Employee1.FirstName + " " + x.Employee1.LastName,
                ApprovedBy = x.Employee.FirstName + " " + x.Employee.LastName,
            });

            
            int recordsTotal = data1.Count();
            return Json(new {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
               data = data1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HandleApproveDataTable()
        {
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            int draw = Convert.ToInt32(Request["draw"]);
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);

            string searchValue = Request["search[value]"];
            var str = Request["order[0][column]"];
            var sortColumn = Request["columns[" + Request["order[0][column]"] + "][name]"];
            var sortColumnDirection = Request["order[0][dir]"];
            IQueryable<Task> data;

            if (roleId !=2)
            {
              data = _db.Tasks.Where(x => x.Employee2.DepartmentId > roleId);
            }
            else
            {
                data = _db.Tasks.Where(x => x.ApproverID == userId);
            }
            

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>
                (x.Employee1.FirstName + " " + x.Employee1.LastName).Contains(searchValue) &&
                (x.Employee.FirstName + " " + x.Employee.LastName).Contains(searchValue) &&
                (x.Employee2.FirstName + " " + x.Employee2.LastName).Contains(searchValue)
                );
            }
            if (!string.IsNullOrEmpty(sortColumn))
            {

                switch (sortColumn)
                {
                    case "TaskDate":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.TaskDate);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.TaskDate);
                        }
                        break;
                    case "EmployeeName":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.Employee2.FirstName + " " + x.Employee2.LastName);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.Employee2.FirstName + " " + x.Employee2.LastName);
                        }
                        break;
                    case "TaskName":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.TaskName);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.TaskName);
                        }
                        break;
                    case "Status":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.Status);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.Status);
                        }
                        break;
                    case "Approver":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.Employee.FirstName + " " + x.Employee.LastName);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.Employee.FirstName + " " + x.Employee.LastName);
                        }
                        break;
                    case "ApprovedOn":
                        if (sortColumnDirection == "asc")
                        {
                            data = data.OrderBy(x => x.ApprovedORRejectedOn);
                        }
                        else
                        {
                            data = data.OrderByDescending(x => x.ApprovedORRejectedOn);
                        }
                        break;
                    default:
                        data = data.OrderByDescending(x => x.TaskDate);
                        break;
                }
               
            }
            else
            {
                data = data.OrderBy(x => x.TaskDate);
            }
            var data1 = data
                 .Skip(start)
                 .Take(length)
                 .Select(x => new {
                     ApprovedDate = x.ApprovedORRejectedOn,
                     x.TaskId,
                     x.TaskDate,
                     x.TaskName,
                     x.TaskDescription,
                     x.Status,
                     x.ModifiedOn,
                     ApprovedOn = x.ApprovedORRejectedOn,
                     Approver = x.Employee1.FirstName + " " + x.Employee1.LastName,
                     ApprovedBy = x.Employee.FirstName + " " + x.Employee.LastName,
                     Approver1Id = x.Employee.DepartmentId,
                     EmployeeName = x.Employee2.FirstName + " " + x.Employee2.LastName,
                     Approve = "",
                     Reject = ""
                 });


            int recordsTotal = data1.Count();
            return Json(new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data1
            }, JsonRequestBehavior.AllowGet);
        }
  
private object GetPropertyValue(object obj, string propertyName)

        {

            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);

        }
    }
}