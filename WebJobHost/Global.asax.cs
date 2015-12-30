namespace WebJobHost
{
    using System.Diagnostics;
    using System.Web;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Do nothing else here, need application class for host.
            Trace.TraceInformation("WebJobHost starting...");
        }
    }
}