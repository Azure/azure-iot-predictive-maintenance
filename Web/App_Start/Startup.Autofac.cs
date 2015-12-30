namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Reflection;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using Common.Configurations;
    using Common.Repository;
    using global::Owin;
    using Services;

    public partial class Startup
    {
        public void ConfigureAutofac(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConfigurationProvider>()
                .As<IConfigurationProvider>();

            builder.RegisterType<TelemetryService>()
                .As<ITelemetryService>();

            builder.RegisterType<DeviceService>()
                .As<IDeviceService>();

            builder.RegisterType<SimulationService>()
                .As<ISimulationService>();

            builder.RegisterType<IotHubRepository>()
                .As<IIotHubRepository>();

            builder.RegisterType<Settings>();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            //Setup Autofac dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //Setup Autofac dependency resolver for WebAPI
            HttpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(HttpConfiguration);
        }
    }
}