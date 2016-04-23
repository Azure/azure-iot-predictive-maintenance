namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Helpers;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
        }

        protected void Application_BeginRequest()
        {
            string cultureName;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = this.Request.Cookies["_culture"];

            if (cultureCookie != null)
            {
                cultureName = cultureCookie.Value;
            }
            else
            {
                // Obtain it from HTTP header AcceptLanguages
                cultureName = this.Request.UserLanguages != null && this.Request.UserLanguages.Length > 0 ? this.Request.UserLanguages[0] : null;
            }

            // Validate culture name
            var culture = CultureHelper.GetClosestCulture(cultureName);

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected void Application_Error()
        {
            Exception error = Server.GetLastError();

            if (error != null)
            {
                Trace.TraceError("Unhandled Exception : {0}", error.Message);
            }
        }
    }
}