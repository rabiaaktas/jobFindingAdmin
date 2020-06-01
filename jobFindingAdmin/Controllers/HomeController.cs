using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jobFindingAdmin.Models;

namespace jobFindingAdmin.Controllers
{
    public class HomeController : Controller
    {
        private AdminEntities db = new AdminEntities();
        // GET: Home
        [UserCheck]
        [Route("Admin/Home/Index")]
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user_account user)
        {
            var us = user.userEmail;
            var ps = user.userPassword;
            var password = Crypt.Encrypt(user.userPassword);
            var data = db.user_admin.Where(x => x.adminEmail == user.userEmail && x.adminPassword == password && x.adminIsActive == "1").FirstOrDefault();
            if (data != null)
            {
                LoginStatus.Current.IsLogin = true;
                LoginStatus.Current.Name = data.adminName;
                LoginStatus.Current.Surname = data.adminSurname;
                LoginStatus.Current.UserId = data.adminAccountId;
                LoginStatus.Current.IsActive = data.adminIsActive;
                var userLog = db.admin_log.Where(x => x.adminAccountID == data.adminAccountId).FirstOrDefault();
                if(userLog == null)
                {
                    admin_log log = new admin_log();
                    log.adminAccountID = data.adminAccountId;
                    log.loginDate = DateTime.Now;
                    string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (ipAddress == "" || ipAddress == null)
                    {
                        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                    }
                    log.loginIp = ipAddress;
                    db.admin_log.Add(log);
                    db.SaveChanges();
                }
                else
                {
                    userLog.loginDate = DateTime.Now;
                    string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (ipAddress == "" || ipAddress == null)
                    {
                        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                    }
                    userLog.loginIp = ipAddress;
                    db.SaveChanges();
                }
             
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Warning = "Kullanıcı adı ve ya şifre hatalı.";
            }
            return View();

        }

        public JsonResult Logout()
        {
            Session.Clear();
            return Json("ok",JsonRequestBehavior.AllowGet);
        }
    }
}