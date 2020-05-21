using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using jobFindingAdmin.Models;
namespace jobFindingAdmin
{
    public class LoginStatus
    {
        public static LoginStatus Current
        {
            get
            {
                LoginStatus loginStatus = (LoginStatus)HttpContext.Current.Session["LoginStatus"];
                if(loginStatus == null)
                {
                    loginStatus = new LoginStatus();
                    HttpContext.Current.Session["LoginStatus"] = loginStatus;
                }
                return loginStatus;
            }
        }

        public bool IsLogin { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Sa { get; set; }
        public int UserId { get; set; }
        public string isActive { get; set; }
        public int Status { get; set; }
        public int userType { get; set; }
        public string PageCategory { get; set; }
        public bool Admin { get; set; }
        public string Ip { get; set; }

    }
}