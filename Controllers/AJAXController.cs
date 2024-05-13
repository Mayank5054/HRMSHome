using HRMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}