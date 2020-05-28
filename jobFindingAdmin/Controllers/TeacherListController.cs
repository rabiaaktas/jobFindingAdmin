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
        [UserCheck]
        public ActionResult Index()
        {
            string rb = "rbakt258.";
            rb = Crypt.Encrypt(rb);
            ViewBag.rb = rb;
            string dc = Crypt.Decrypt(rb);
            ViewBag.dc = dc;
            return View();
        }

        [UserCheck]
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
                               where ua.userTypeID == 3
                               select new { ua.userAccountId, ua.userEmail, ua.firstName, ua.lastName, te.degree, ua.userIsActive, ua.userIsConfirmed, ustype.userTypeId, ustype.user_type_name };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    custData = custData.OrderBy(sortColumn + " " + sortColumnDir);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lengthofSearch = searchValue.Length;
                    custData = custData.Where(x => x.firstName.Substring(0, lengthofSearch).Equals(searchValue) || x.lastName.Substring(0, lengthofSearch).Equals(searchValue) || x.userEmail.Substring(0, lengthofSearch).Equals(searchValue));
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
        [HttpPost]
        public JsonResult userDeactivate(user_account user)
        {
            var selectedUser = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            selectedUser.userIsActive = "0";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);


        }

        [UserCheck]
        [HttpPost]
        public JsonResult userActivate(user_account user)
        {
            var selectedUser = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            selectedUser.userIsActive = "1";
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [UserCheck]
        [HttpPost]
        public JsonResult changeUserRole(user_account user)
        {
            var selectedUser = db.user_account.Where(x => x.userAccountId == user.userAccountId).FirstOrDefault();
            var type = user.userTypeID;
            selectedUser.userTypeID = type;
            db.SaveChanges();
            var teacher = db.user_teacher.Where(x => x.userAccountID == selectedUser.userAccountId).FirstOrDefault();
            db.user_teacher.Remove(teacher);
            db.SaveChanges();
            if (user.userTypeID != 1)
            {
                var userT = new user_student();
                userT.userAccountID = selectedUser.userAccountId;
                db.user_student.Add(userT);
                db.SaveChanges();
            }
            else
            {
                var admin = new user_admin();
                admin.adminAccountId = selectedUser.userAccountId;
                admin.adminEmail = selectedUser.userEmail;
                admin.adminName = selectedUser.firstName;
                admin.adminSurname = selectedUser.lastName;
                admin.adminRegisterDate = DateTime.Now;
                admin.adminPassword = selectedUser.userPassword;
                admin.adminIsActive = selectedUser.userIsActive;
                db.user_admin.Add(admin);
                db.user_account.Remove(selectedUser);
                db.SaveChanges();
            }
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }


        [UserCheck]
        [HttpPost]
        public ActionResult UserInfo(int? id)
        {
            var teach = db.user_teacher.FirstOrDefault(x => x.userAccountID == id);
            ViewBag.Interested = teach.interestAreas;
            ViewBag.Degree = teach.degree;
            return PartialView(db.user_account.FirstOrDefault(x => x.userAccountId == id));
        }
    }

    
}