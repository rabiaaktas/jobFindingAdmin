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
    public class SettingsController : Controller
    {

        private AdminEntities db = new AdminEntities();
        // GET: Settings
        [UserCheck]
        public ActionResult PersonalInfo()
        {
            var id = LoginStatus.Current.UserId;
            return View(db.user_admin.Where(x => x.adminAccountId == id).FirstOrDefault());
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalInfo(user_admin ad)
        {
            if (ModelState.IsValid)
            {
                var admin = db.user_admin.Where(x => x.adminAccountId == ad.adminAccountId).FirstOrDefault();
                if(admin != null)
                {
                    admin.adminName = ad.adminName;
                    admin.adminSurname = ad.adminSurname;
                    admin.adminEmail = ad.adminEmail;
                    db.SaveChanges();
                    ViewBag.Success = "Değişikliler başarı ile yapıldı.";
                }
                else
                {
                    ViewBag.Warning = "Kullanıcı bulunamadı. Tekrar deneyiniz.";
                }
            }
            else
            {
                ViewBag.Warning = "Değişiklikler yapılamadı. Tekrar deneyiniz.";
            }
            return View(ad);
        }

        [UserCheck]
        public ActionResult ChangePass()
        {
            var id = LoginStatus.Current.UserId;
            var admin = db.user_admin.Where(x => x.adminAccountId == id).FirstOrDefault();
            var setModel = new SettingModel();
            if (admin != null)
            {
                setModel.ID = admin.adminAccountId;
                var old = Crypt.Decrypt(admin.adminPassword);
                setModel.OldPassword = old;
            }
            return View(setModel);
        }

        [UserCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(SettingModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = db.user_admin.Where(x => x.adminAccountId == model.ID).FirstOrDefault();
                if(admin != null)
                {
                    var oldPass = Crypt.Decrypt(admin.adminPassword);
                    if(oldPass == model.NewPassword)
                    {
                        ViewBag.Warning = "Yeni parola eskisi ile aynı olamaz. Tekrar deneyiniz.";
                    }
                    else
                    {
                        var newPass = Crypt.Encrypt(model.NewPassword);
                        admin.adminPassword = newPass;
                        model.NewPassword = "";
                        model.ConfirmPassword = "";
                        db.SaveChanges();
                        ViewBag.Success = "Parola başarı ile değiştirildi.";
                    }
                    

                }
                else
                {
                    ViewBag.Warning = "Kullanıcı bulunamadı. Tekrar deneyiniz.";
                }
            }
            else
            {
                ViewBag.Warning = "Parola değiştirilemedi. Tekrar deneyiniz.";
            }
            return View(model);
        }
        


    }
}