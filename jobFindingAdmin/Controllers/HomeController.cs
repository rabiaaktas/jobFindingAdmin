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
        public ActionResult Index()
        {
            return View();
        }
    }
}