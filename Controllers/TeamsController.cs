using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Filters;
using HRMS.Models;
using Newtonsoft.Json;
namespace HRMS.Controllers
{
    [LoginFilter]
    public class TeamsController : Controller
    {
        MayankEntities _db;
        public TeamsController()
        {
            _db = new MayankEntities();
        }
        // GET: Teams
        [@Authorize(new string[] { "Director" })]
        public ActionResult CreateTeam()
        {
            List<SelectListItem> _listOfManagers = _db.Employees
                .Where(x => x.DepartmentId == 2)
                .Select(x=> new SelectListItem
                {
                    Text = x.FirstName + " " + x.LastName,
                    Value = x.EmployeeId.ToString()
                }).ToList();

            ViewBag.Managers = _listOfManagers;
            return View();
        }

        [@Authorize(new string[] { "Director" })]
        [HttpPost]
        public ActionResult CreateTeam(Team _team, int[] selectedEmployee)
        {
            List<SelectListItem> _listOfManagers = _db.Employees
                    .Where(x => x.DepartmentId == 2)
                    .Select(x => new SelectListItem
                    {
                        Text = x.FirstName + " " + x.LastName,
                        Value = x.EmployeeId.ToString()
                    }).ToList();
            ViewBag.Managers = _listOfManagers;
            if (ModelState.IsValid && selectedEmployee!=null && selectedEmployee.Length >= 2)
            {
                Team team = _db.Teams.Where(x=>x.TeamLeader==_team.TeamLeader).FirstOrDefault();
                if(team == null)
                {
                    _team.CreationDate = DateTime.Now;
                    _db.Teams.Add(_team);
                    _db.SaveChanges();
                    foreach (int i in selectedEmployee)
                    {
                        TeamsDetail detail = new TeamsDetail();

                        detail.TeamId = _team.TeamId;
                        detail.EmployeeId = i;
                        detail.AssignedRole = "Programmer";
                        _db.TeamsDetails.Add(detail);
                    }
                    _db.SaveChanges();
                    TempData["TeamCreated"] = "Team Created Successfully";
                    return RedirectToAction("GetMyTeams");
                }
                else
                {
                    ViewBag.TeamLeaderHasATeam = "Team Leader Has Already Assign A team";
                    return View();
                }
            
            }
            else
            {
                
                ViewBag.NotEnoughEmployees = "Not Enough Employees Are There";
               
                return View();
            }
        }

        
        public ActionResult GetMyTeams()
        {
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            List<Team> teams;
            if (roleId == 1)
            {
                teams = _db.Teams.Include("Employee").ToList();
            }
            else if(roleId == 2)
            {
                teams = _db.Teams.Where(x => x.TeamLeader == userId).ToList();
            }
            else
            {
                Employee employee = _db.Employees.Find(userId);
                
                teams = _db.Teams.Where(x => x.TeamLeader == employee.ReportingPerson).ToList();
            }

            return View(teams);
        }

        [@Authorize(new string[] { "Director", "Manager","Employee" })]
        public ActionResult GetTeamDetails(int id)
        { 
            Team _teamDetail = _db.Teams.
               Include("TeamsDetails").Where(x => x.TeamId==id).FirstOrDefault();

            //List<TeamsDetail> _teamToPass = _teamDetail.TeamsDetails.Where(x=>x.Employee.isDeleted == false).ToList();
            //_teamDetail.TeamsDetails = _teamToPass;

            return View(_teamDetail);
        }
        [@Authorize(new string[] {"Manager" })]
        public ActionResult EditAssignedRole(int id)
        {
            TeamsDetail team = _db.TeamsDetails
                .Include("Employee")
                .Include("Team")
                .Where(x => x.TeamMemberId == id)
                .FirstOrDefault();
            return View(team);
        }

        [@Authorize(new string[] { "Manager" })]
        [HttpPost]
        public ActionResult EditAssignedRole(TeamsDetail _td)
        {
            Team td = _db.Teams.Where(x=>x.TeamId== _td.TeamId).FirstOrDefault();
            if(td != null)
            {
                TeamsDetail _td1 = _db.TeamsDetails.Where(x=>x.TeamMemberId== _td.TeamMemberId).FirstOrDefault();
                if(_td1 != null && td.TeamLeader == _td1.Team.TeamLeader)
                {
                    _td1.AssignedRole = _td.AssignedRole;

                }
                _db.SaveChanges();
        
                return RedirectToAction("GetMyTeams");
            }
            else
            {
                return RedirectToAction("GetMyTeams");
            }
         
        }

 
        public ActionResult DeleteTeam(int id)
        {
            int userId = int.Parse(Session["userId"].ToString());
            int roleId = int.Parse(Session["RoleId"].ToString());
            if (roleId == 1)
            {
                List<TeamsDetail> teams = _db.TeamsDetails.Where(x => x.TeamId == id).ToList();

                foreach (TeamsDetail i in teams)
                {
                    _db.TeamsDetails.Remove(i);
                }
                Team team = _db.Teams.Find(id);
                if (team != null)
                {
                    _db.Teams.Remove(team);
                }
                _db.SaveChanges();
                return Json(new { status = "Success", message = "Team Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Failure", message = "UnAuthorized Action" }, JsonRequestBehavior.AllowGet);
            }
          
        }

  
        public ActionResult GetEmployeeByReportingPerson(int id)
        {
            List<SelectListItem> _listOfEmp = _db.Employees.Where(x => x.ReportingPerson == id).Select(x => new SelectListItem
            {
                Text = x.FirstName + " " + x.LastName,
                Value = x.EmployeeId.ToString()
            }).ToList();

            string jsonData = JsonConvert.SerializeObject(_listOfEmp);
            return Json(new { status = "Success",Employees = jsonData },JsonRequestBehavior.AllowGet);
        }
    }
}