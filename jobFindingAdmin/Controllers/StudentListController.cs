using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Mail;
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
                               where ua.userTypeID == 2
                               select new { ua.userAccountId, ua.userEmail, ua.firstName, ua.lastName, ua.userPhone, ua.userIsActive, ua.userIsConfirmed, ustype.userTypeId, ustype.user_type_name };
                if(!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.firstName.Substring(0, lengthofSearch).Equals(searchValue) || x.lastName.Substring(0,lengthofSearch).Equals(searchValue) || x.userEmail.Substring(0,lengthofSearch).Equals(searchValue));
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

        [HttpPost]
        public JsonResult userDeactivate(user_account user)
        {
            var selectedUser = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            selectedUser.userIsActive = "0";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);

           
        }

        [HttpPost]
        public JsonResult userActivate(user_account user)
        {
            var selectedUser = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            selectedUser.userIsActive = "1";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult changeUserRole(user_account user)
        {
            var selectedUser = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            var type = user.userTypeID;
            selectedUser.userTypeID = type;
            db.SaveChanges();
            var student = db.user_student.Where(x => x.userAccountID == selectedUser.userAccountId).FirstOrDefault();
            db.user_student.Remove(student);
            db.SaveChanges();
            if (user.userTypeID != 1)
            {
                var userT = new user_teacher();
                userT.userAccountID = selectedUser.userAccountId;
                db.user_teacher.Add(userT);
                db.SaveChanges();
            }
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendEmail(user_account user)
        {
            var selected = db.user_account.FirstOrDefault(x => x.userAccountId == user.userAccountId);
            var email = selected.userEmail;
            SmtpClient sc = new SmtpClient();
            sc.Port = 587;
            sc.Host = "";
            sc.EnableSsl = false;
            sc.Credentials = new NetworkCredential("rabia-aktas-98@hotmail.com ", "");
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.Subject = "Kullanıcı Email Aktivasyonu";
            mail.IsBodyHtml = false;
            mail.Body = "";
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}