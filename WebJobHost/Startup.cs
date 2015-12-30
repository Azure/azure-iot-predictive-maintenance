using Microsoft.Owin;
using WebJobHost;

[assembly: OwinStartup(typeof(Startup))]

namespace WebJobHost
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            // Do nothing on startup
        }
    }
}