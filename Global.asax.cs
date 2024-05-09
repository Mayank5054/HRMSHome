using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading;
using System.Reflection;
using System.Resources;
namespace HRMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string userLaanguage = "hi";
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hi-IN");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hi-IN");
            ResourceManager rm = new ResourceManager("HRMS.Resources", Assembly.GetExecutingAssembly());
          
        }
    }
}
