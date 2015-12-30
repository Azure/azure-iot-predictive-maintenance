namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Web.Http;
    using Common.Configurations;
    using global::Owin;
    using Owin;

    public partial class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration = new HttpConfiguration();
            ConfigurationProvider configProvider = new ConfigurationProvider();

            app.Use<EnforceHttpsMiddleware>();

            ConfigureAuth(app, configProvider);
            ConfigureAutofac(app);
            ConfigureWebApi(app);
        }
    }
}