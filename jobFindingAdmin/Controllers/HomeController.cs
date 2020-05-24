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
            //var password = Crypt.Encrypt(user.userPassword);
            var data = db.user_account.Where(x => x.userEmail == user.userEmail && x.userPassword == user.userPassword && x.userIsActive == "1" && x.userTypeID == 1).FirstOrDefault();
            if (data != null)
            {
                LoginStatus.Current.IsLogin = true;
                LoginStatus.Current.Name = data.firstName;
                LoginStatus.Current.Surname = data.lastName;
                LoginStatus.Current.UserId = data.userAccountId;
                LoginStatus.Current.UserType = data.userTypeID;
                LoginStatus.Current.IsActive = data.userIsActive;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Warning = "Kullanıcı adı ve ya şifre hatalı.";
            }
            return View();

        }

        //public ActionResult Logout()
        //{
        //    Session.Clear();
        //    return RedirectToAction("Index","Home");
        //}

        public JsonResult Logout()
        {
            Session.Clear();
            return Json("ok",JsonRequestBehavior.AllowGet);
        }
    }
}