using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using jobFindingAdmin.Models;


namespace jobFindingAdmin.Controllers
{
    public class TeacherListController : Controller
    {
        
        private AdminEntities db = new AdminEntities();
        // GET: TeacherList
        public ActionResult Index()
        {
            string rb = "rbakt258.";
            rb = Crypt.Encrypt(rb);
            ViewBag.rb = rb;
            string dc = Crypt.Decrypt(rb);
            ViewBag.dc = dc;
            return View();
        }

        public ActionResult LoadTeacData()
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
                               join te in db.user_teacher on ua.userAccountId equals te.userAccountID
                               join ustype in db.user_type on ua.userTypeID equals ustype.userTypeId
                               select new { ua.userAccountId, ua.userEmail, te.teacfirstName, te.teaclastName, ua.userBday, ua.userPhone, ua.userAddress, ua.userIsActive, ua.userIsConfirmed, ustype.userTypeId, ustype.user_type_name };
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.teacfirstName.Substring(0, lengthofSearch).Equals(searchValue) || x.teaclastName.Substring(0, lengthofSearch).Equals(searchValue) || x.userEmail.Substring(0, lengthofSearch).Equals(searchValue));
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