namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob
{
    using Autofac;
    using Common.Configurations;
    using Common.Execution;
    using Common.Repository;
    using DataInitialization;

    public sealed class SimulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ShutdownFileWatcher>()
                .As<IShutdownFileWatcher>()
                .SingleInstance();

            builder.RegisterType<ConfigurationProvider>()
                .As<IConfigurationProvider>()
                .SingleInstance();

            builder.RegisterType<IotHubRepository>()
                .As<IIotHubRepository>();

            builder.RegisterType<SecurityKeyGenerator>()
                .As<ISecurityKeyGenerator>();

            builder.RegisterType<VirtualDeviceTableStorage>()
                .As<IVirtualDeviceStorage>();

            builder.RegisterType<DataInitializer>()
                .As<IDataInitializer>();
        }
    }
}