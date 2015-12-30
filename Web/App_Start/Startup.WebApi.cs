namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Web.Http;
    using global::Owin;

    public partial class Startup
    {
        public void ConfigureWebApi(IAppBuilder app)
        {
            app.UseWebApi(HttpConfiguration);

            HttpConfiguration.MapHttpAttributeRoutes();
        }
    }
}