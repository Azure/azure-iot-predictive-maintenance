// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Web.Http;
    using Configurations;
    using global::Owin;

    public partial class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration = new HttpConfiguration();
            ConfigurationProvider configProvider = new ConfigurationProvider();

            this.ConfigureAuth(app, configProvider);
            this.ConfigureAutofac(app);
            //this.ConfigureWebApi(app);
        }
    }
}