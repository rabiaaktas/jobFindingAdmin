using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class UniversityController : Controller
    {

        private AdminEntities db = new AdminEntities();
        // GET: Universty
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

                var custData = from uni in db.universities
                               select new { uni.universityId, uni.universityName };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.universityName.Substring(0, lengthofSearch).Equals(searchValue));
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
            return View(db.universities.FirstOrDefault(x => x.universityId == id));
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(universities uni)
        {
            var selectedUni = db.universities.FirstOrDefault(x => x.universityId == uni.universityId);
            if (selectedUni != null)
            {
                selectedUni.universityName = uni.universityName;
                db.SaveChanges();
                return RedirectToAction("Index", "University");
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
        public ActionResult Add(universities uni)
        {
            if (uni.universityName != null)
            {
                var newUni = new universities();
                newUni.universityName = uni.universityName;
                db.universities.Add(newUni);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "University");
        }
    }
}