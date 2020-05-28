using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using jobFindingAdmin.Models;
using jobFindingAdmin.Models.HelperModel;

namespace jobFindingAdmin.Controllers
{
    public class ChangeController : Controller
    {

        private AdminEntities db = new AdminEntities();
        // GET: Change
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(user_admin user)
        {
            bool exist = isEmailExist(user.adminEmail);
            if (exist == false)
            {
                ViewBag.Warning = "E-mail adresi bulunamadı.";    
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                var verifyUrl = "Change/ResetPassword/" + token;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var selected = db.user_admin.Where(x => x.adminEmail == user.adminEmail).FirstOrDefault();
                selected.adminResetCode = token;
                db.SaveChanges();
                var subject = "Parola Yenileme Talebi";
                var body = "Merhaba " + selected.adminName + ", <br/> Hesabınız için parola yenileme talebinde bulundunuz. Aşağıdaki linke tıklayarak parolanızı yenileyebilirsiniz." + "<br/><br/><a href='" + link + "'>Buraya Tıklayınız</a> <br/><br/>" + "Teşekkürler";
                sendEmail(selected.adminEmail, body, subject);
                ViewBag.Success = "Parola yenileme linki e-adresinize gönderildi.";
            }
            return View();
        }

        public bool isEmailExist(string email)
        {
            bool exist = true;
            var user = db.user_admin.Where(x => x.adminEmail == email).FirstOrDefault();
            if(user == null)
            {
                exist = false;
            }
            return exist;
        }

        [NonAction]
        public void sendEmail(string email, string body, string subject)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("aktasrabiaa@gmail.com");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            using (SmtpClient sc = new SmtpClient())
            {
                sc.Port = 587;
                sc.Host = "smtp.live.com";
                sc.EnableSsl = true;
                sc.Credentials = new NetworkCredential("rabia-aktas-98@hotmail.com", "rbakt258.");
                sc.UseDefaultCredentials = false;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Send(mail);

            }


        }

        
        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            var user = db.user_admin.Where(x => x.adminResetCode == id).FirstOrDefault();
            if(user != null)
            {
                resetPassword us = new resetPassword();
                us.ResetCode = id;
                return View(us);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(resetPassword reset)
        {
            if (ModelState.IsValid)
            {
                var user = db.user_admin.Where(x => x.adminResetCode == reset.ResetCode).FirstOrDefault();
                if(user != null)
                {
                    user.adminPassword = reset.NewPassword;
                    user.adminResetCode = "";
                    db.SaveChanges();
                    ViewBag.Success = "Parola başarıyla değiştirildi.";
                }
            }
            else
            {
                ViewBag.Warning = "Parola değiştirilemedi. Tekrar deneyiniz.";
            }
            return View(reset);
        }
    }
}