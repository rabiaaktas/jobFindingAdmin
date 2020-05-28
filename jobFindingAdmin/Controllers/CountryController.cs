using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class CountryController : Controller
    {

        private AdminEntities db = new AdminEntities();
        // GET: Country
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

                var custData = from co in db.countries
                               select new { co.countryId, co.countryName };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.countryName.Substring(0, lengthofSearch).Equals(searchValue));
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
            return View(db.countries.FirstOrDefault(x => x.countryId == id));
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(countries country)
        {
            var bs = db.countries.FirstOrDefault(x => x.countryId == country.countryId);
            if (bs != null)
            {
                bs.countryName = country.countryName;
                db.SaveChanges();
                return RedirectToAction("Index", "Country");
            }
            else
            {
                ViewBag.Warning = "Düzenleme gerçekleştirilemedi.";
                return View();
            }
        }
    }
}