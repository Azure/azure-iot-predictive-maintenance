using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Execution;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Repository;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.DataInitialization;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob
{
    using Configurations;

    public sealed class SimulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ShutdownFileRunner>()
                .As<IShutdownFileRunner>()
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
