using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using jobFindingAdmin.Models;

namespace jobFindingAdmin
{
    public class Tools
    {
        private AdminEntities db = new AdminEntities();
        public void Auth(int userID,string isActive, int userType)
        {
            var admin = db.user_account.Where(x => x.userAccountId == userID && x.userIsActive == isActive && x.userTypeID == userType).FirstOrDefault();
            if(admin == null)
            {
                HttpContext.Current.Response.Redirect(MvcApplication.FilePath + "", true);
            }
        }
    }
}