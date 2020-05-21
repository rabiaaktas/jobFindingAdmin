using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jobFindingAdmin
{
    public class UserCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!LoginStatus.Current.IsLogin)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"controller","Home" }, {"action","Login"}
                });
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class PermissionCheck : ActionFilterAttribute
    {
        public string pageCategory { get; set; }


    }
}