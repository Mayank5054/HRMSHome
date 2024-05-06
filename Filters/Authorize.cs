using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc ;
using System.Web.Mvc.Filters;
using System.Web.Security;

namespace HRMS.Filters
{
  
    public class Authorize : FilterAttribute, IAuthorizationFilter
    {
         string[] _roles;
        public Authorize(string[] roles)
        {
            _roles = roles;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if ( Array.IndexOf(_roles,filterContext.HttpContext.Session["Role"]) == -1 )
            {
                filterContext.Result = new RedirectResult("~/Authentication/Error404");
            }
        }
    }
}