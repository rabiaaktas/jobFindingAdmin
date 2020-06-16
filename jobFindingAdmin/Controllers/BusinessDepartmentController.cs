using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class BusinessDepartmentController : Controller
    {
        private AdminEntities db = new AdminEntities();
        // GET: BusinessDepartment
        [UserCheck]
        public ActionResult Index()
        {
            return View();
        }

        [UserCheck]
        public ActionResult LoadData()
        {
            try
            {
                var draw = Request.Form.GetValues("draw");
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var custData = from bd in db.business_departments
                               select new { bd.businessDepId, bd.businessDepName };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.businessDepName.Substring(0, lengthofSearch).Equals(searchValue));
                }

                recordsTotal = custData.Count();

                var data = custData.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [UserCheck]
        public ActionResult Edit(int? id)
        {
            return View(db.business_departments.FirstOrDefault(x => x.businessDepId == id));
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(business_departments bd)
        {
            var selectedBd = db.business_departments.FirstOrDefault(x => x.businessDepId == bd.businessDepId);
            if (selectedBd != null)
            {
                selectedBd.businessDepName = bd.businessDepName;
                db.SaveChanges();
                return RedirectToAction("Index", "BusinessDepartment");
            }
            else
            {
                ViewBag.Warning = "Düzenleme gerçekleştirilemedi.";
                return View();
            }
        }

        [UserCheck]
        public ActionResult Add()
        {
            return View();
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(business_departments bd)
        {
            if (bd.businessDepName != null)
            {
                var newBD = new business_departments();
                newBD.businessDepName = bd.businessDepName;
                db.business_departments.Add(newBD);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "BusinessDepartment");
        }
    }
}