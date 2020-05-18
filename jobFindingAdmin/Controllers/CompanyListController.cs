using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class CompanyListController : Controller
    {
        private AdminEntities db = new AdminEntities();
        // GET: CompanyList
        //[Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadCompanyTable()
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

                var custData = from co in db.company join bs in db.business_stream on co.businessID equals bs.businessId
                               select new { co.companyId, co.companyName, co.companyEmail, bs.businessId, bs.businessName, co.isCompanyActive };

                if(!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir))){
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.companyName.Substring(0,lengthofSearch).Equals(searchValue) || x.businessName.Substring(0,lengthofSearch).Equals(searchValue));
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

        [HttpPost]
        public ActionResult Details(int? id)
        {
            return PartialView(db.company.FirstOrDefault(x => x.companyId == id));
        }

        [HttpPost]
        public JsonResult companyDeactivate(company company)
        {
            var selected = db.company.Where(x => x.companyId == company.companyId).FirstOrDefault();
            selected.isCompanyActive = "0";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult companyActivate(company company)
        {
            var selected = db.company.Where(x => x.companyId == company.companyId).FirstOrDefault();
            selected.isCompanyActive = "1";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}