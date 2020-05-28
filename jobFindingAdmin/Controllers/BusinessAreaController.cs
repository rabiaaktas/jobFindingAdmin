using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class BusinessAreaController : Controller
    {

        private AdminEntities db = new AdminEntities();
        // GET: BusinessArea
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

                var custData = from ba in db.business_stream
                               select new { ba.businessId, ba.businessName };
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.businessName.Substring(0, lengthofSearch).Equals(searchValue));
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
        public ActionResult Add()
        {
            return View();
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(business_stream bs)
        {
            if(bs.businessName != null)
            {
                var newbs = new business_stream();
                newbs.businessName = bs.businessName;
                db.business_stream.Add(newbs);
                db.SaveChanges();
            }
            return RedirectToAction("Index","BusinessArea");
        }

        [UserCheck]
        public ActionResult Edit(int? id)
        {
            return View(db.business_stream.FirstOrDefault(x => x.businessId == id));
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(business_stream bst)
        {
            var bs = db.business_stream.FirstOrDefault(x => x.businessId == bst.businessId);
            if (bs != null)
            {
                bs.businessName = bst.businessName;
                db.SaveChanges();
                return RedirectToAction("Index", "BusinessArea");
            }
            else
            {
                ViewBag.Warning = "Düzenleme gerçekleştirilemedi.";
                return View();
            }
        }
    }
}