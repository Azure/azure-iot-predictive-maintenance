// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using Autofac;
    using global::Owin;

    public partial class Startup
    {
        public void ConfigureAutofac(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
        }
    }

    public sealed class WebAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

        }
    }
}