using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using jobFindingAdmin.Models;
using jobFindingAdmin.Models.HelperModel;

namespace jobFindingAdmin.Controllers
{
    public class StudentListController : Controller
    {
        private AdminEntities db = new AdminEntities();
        // GET: StudentList
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadTableData()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var custData = from ua in db.user_account
                               join st in db.user_student on ua.userAccountId equals st.userAccountID
                               join ustype in db.user_type on ua.userTypeID equals ustype.userTypeId
                               select new { ua.userAccountId, ua.userEmail, st.stufirstName, st.stulastName, ua.userBday, ua.userPhone, ua.userAddress, ua.userIsActive, ua.userIsConfirmed, ustype.userTypeId, ustype.user_type_name };
                if(!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.stufirstName.Substring(0, lengthofSearch).Equals(searchValue) || x.stulastName.Substring(0,lengthofSearch).Equals(searchValue) || x.userEmail.Substring(0,lengthofSearch).Equals(searchValue));
                }

                recordsTotal = custData.Count();

                var data = custData.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}