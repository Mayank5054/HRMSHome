using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Security;
using HRMS.Filters;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
namespace HRMS.Controllers
{
    public class JWTController : Controller
    {
        MayankEntities _db;
        public JWTController()
        {
            _db = new MayankEntities();
        }
        // GET: JWT

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginUser _loginUser)
        {
            Console.WriteLine(_loginUser);
            Employee employee = _db.Employees.Where(x => (x.Email == _loginUser.Email && x.Password == _loginUser.Password)).FirstOrDefault();
            if (employee != null)
            {
                object _obj = new 
                {
                    Email = employee.Email,
                    RoleId = employee.Role.RoleId,
                    userId = employee.EmployeeId
                };
                string token = JWT.GenerateJWTToken(_obj);

                return Json(new { status = "Success", AuthenticationToken = token });
            }
            else
            {
                return Json(new { status = "Failure", message = "User Not Found" });
            }

        }

        [JWTLoginFilter]
        public ActionResult GetTask()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(Request.Cookies["jwtToken"].Value);
            string _jy = JWT.Base64UrlDecode(jwtToken.EncodedPayload);
            JObject _ja = JObject.Parse(_jy);
            string uniqueName = (string)_ja["unique_name"];
            JObject _ja2 = JObject.Parse(uniqueName);
            string email = (string)_ja2["Email"];
            int roleId = (int)_ja2["RoleId"];
            int userId = (int)_ja2["userId"];

            if (roleId != 2)
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
     
                List<HRMS.Models.Task> _taskData = _db.Tasks
                          .Where(x => x.ApproverID == userId)
                          .Take(2)
                          .ToList();
                return View(_taskData);
            }
            
        }
        [JWTLoginFilter]
        public ActionResult AddTask()
        {
            
            return View();
        }
        [JWTLoginFilter]
        [HttpPost]
        public ActionResult AddTask(Employee _emp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(Request.Cookies["jwtToken"].Value);
            string _jy = JWT.Base64UrlDecode(jwtToken.EncodedPayload);
            JObject _ja = JObject.Parse(_jy);
            string uniqueName = (string)_ja["unique_name"];
            JObject _ja2 = JObject.Parse(uniqueName);
            string email = (string)_ja2["Email"];
            int roleId = (int)_ja2["RoleId"];
            int userId = (int)_ja2["userId"];

            return Json(new { });
        }


        public ActionResult HandleApproveDataTable()
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(Request.Cookies["jwtToken"].Value);
            string _jy = JWT.Base64UrlDecode(jwtToken.EncodedPayload);
            JObject _ja = JObject.Parse(_jy);
            string uniqueName = (string)_ja["unique_name"];
            JObject _ja2 = JObject.Parse(uniqueName);
            string email = (string)_ja2["Email"];
            int roleId = (int)_ja2["RoleId"];
            int userId = (int)_ja2["userId"];

            int draw = Convert.ToInt32(Request["draw"]);
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);

            string searchValue = Request["search[value]"];
            var str = Request["order[0][column]"];
            var sortColumn = Request["columns[" + Request["order[0][column]"] + "][name]"];
            var sortColumnDirection = Request["order[0][dir]"];
            IQueryable<Task> data;

            if (roleId != 2)
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
                (x.Employee1.FirstName + " " + x.Employee1.LastName).Contains(searchValue) ||
                (x.Employee.FirstName + " " + x.Employee.LastName).Contains(searchValue) ||
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

        
    }
}