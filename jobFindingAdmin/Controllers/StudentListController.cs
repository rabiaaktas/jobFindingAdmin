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
        [UserCheck]

        public ActionResult Index()
        {
            return View();
        }

        [UserCheck]
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
                               select new { ua.userAccountId, ua.userEmail, ua.firstName, ua.lastName, st.statusStd, ua.userIsActive, ua.userIsConfirmed, ustype.userTypeId, ustype.user_type_name };
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
        public JsonResult SendActivationEmail(user_account user)
        {
            var token = Guid.NewGuid().ToString();
            var activationUrl = "Verify/" + token;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, activationUrl);
            var selected = db.user_account.FirstOrDefault(x => x.userAccountId == user.userAccountId);
            selected.mailActivationCode = token;
            db.SaveChanges();
            var subject = "E-mail Aktivasyonu";
            var body = "Merhaba " + selected.firstName + ", <br/> E-mail aktivasyonu için aşağıdaki linke tıklayınız." + "<br/><br/><a href='" + link + "'>Buraya Tıklayınız</a> <br/><br/>" + "Teşekkürler";
            sendEmail(selected.userEmail, body, subject);
            return Json("ok",JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public void sendEmail(string email, string body, string subject)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mail-adddres");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            using (SmtpClient sc = new SmtpClient())
            {
                sc.Port = 587;
                sc.Host = "smtp.live.com";
                sc.EnableSsl = true;
                sc.Credentials = new NetworkCredential("mail-address", "password");
                sc.UseDefaultCredentials = false;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Send(mail);

            }
        }

        [UserCheck]
        [HttpPost]
        public ActionResult UserInfo(int? id)
        {
            var stu = db.user_student.FirstOrDefault(x => x.userAccountID == id);
            var sector = db.business_stream.FirstOrDefault(x => x.businessId == stu.intrestedSectorId);
            ViewBag.Interested = sector.businessName;
            ViewBag.Status = stu.statusStd;
            return PartialView(db.user_account.FirstOrDefault(x => x.userAccountId == id));
        }

        [UserCheck]
        public ActionResult Edit(int? id)
        {
            return View(db.user_account.FirstOrDefault(x => x.userAccountId == id));
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(user_account user)
        {
            var stu = db.user_account.FirstOrDefault(x => x.userAccountId == user.userAccountId);
            if (stu != null)
            {
                stu.firstName = user.firstName;
                stu.lastName = user.lastName;
                stu.userAddress = user.userAddress;
                stu.userPhone = user.userPhone;
                db.SaveChanges();
                return RedirectToAction("Index", "StudentList");
            }
            else
            {
                ViewBag.Warning = "Düzenleme gerçekleştirilemedi.";
                return View();
            }
        }
    }
}