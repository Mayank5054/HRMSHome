using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using HRMS.Filters;
using HRMS.Models;
namespace HRMS.Controllers
{
    [LoginFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [@Authorize(new string[] { "Manager", "Employee" })]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

g CRUDPractice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRUDPractice.Controllers
{
    public class HomeController : Controller
    {
        MayankEntities _db;
        public HomeController()
        {
            _db = new MayankEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return PartialView("Create");
        }


        [HttpPost]
        public ActionResult Create(AllField _all)
        {
            if (ModelState.IsValid)
            {
                _db.AllFields.Add(_all);
                _db.SaveChanges();
                return Json(new { data = _all });
            }
            else
            {
                return Json(new { data = _all });
            }

        }

        public ActionResult AllRecords()
        {
            List<AllField> _list = _db.AllFields.ToList();
            return View(_list);
        }

        public ActionResult Edit(int recordId)
        {
            ViewBag.operation = "Edit";
            AllField _record = _db.AllFields.Find(recordId);
            return PartialView("Create", _record);
        }
        [HttpPost]
        public ActionResult Edit(AllField _record)
        {
            //ViewBag.operation = "Edit";
            //AllField _record = _db.AllFields.Find(recordId);
            _db.AllFields.AddOrUpdate(_record);
            _db.SaveChanges();
            return Json(new { data = _record });
        }

    }
}
