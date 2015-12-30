namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

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