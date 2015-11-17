// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using Autofac;
<<<<<<< HEAD
=======
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using Configurations;
>>>>>>> az_local
    using global::Owin;

    public partial class Startup
    {
        public void ConfigureAutofac(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
<<<<<<< HEAD
        }
    }

    public sealed class WebAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

=======

            builder.RegisterType<ConfigurationProvider>().As<IConfigurationProvider>();

            var container = builder.Build();

            //Setup Autofac dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //Setup Autofac dependency resolver for WebAPI
            HttpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            //app.UseAutofacWebApi(HttpConfiguration);
>>>>>>> az_local
        }
    }
}