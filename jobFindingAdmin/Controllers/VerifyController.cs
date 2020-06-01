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
    public class VerifyController : Controller
    {
        private AdminEntities db = new AdminEntities();
        // GET: Verify
        public ActionResult Activate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Warning = "Geçersiz aktivasyon kodu.";
            }
            else
            {
                var user = db.user_account.Where(x => x.mailActivationCode == id).FirstOrDefault();
                if (user != null)
                {
                    user.mailActivationCode = "";
                    user.userIsConfirmed = "1";
                    db.SaveChanges();
                    ViewBag.Success = "Aktivasyon başarıyla tamamlandı";
                }
                else
                {
                    ViewBag.Warning = "Geçersiz aktivasyon kodu.";
                }
            }
            return View();

        }
    }
}