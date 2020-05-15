using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class PostListController : Controller
    {
        private AdminEntities db = new AdminEntities();
        // GET: PostList
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadPostList()
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

                var custData = from jp in db.job_post join jl in db.job_location on jp.jobLocationID equals jl.jobLocationId
                               join jt in db.job_type on jp.jobTypeID equals jt.jobTypeId join co in db.company on jp.companyID equals co.companyId
                               select new { jp.jobPostId, co.companyName, jp.jobPostTitle, jp.postCreatedDate, jp.department, jt.jobTypeName, jl.country, jl.streetAddress, jp.isActivePost};
                if(!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.companyName.Substring(0, lengthofSearch).Equals(searchValue));
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
    }
}