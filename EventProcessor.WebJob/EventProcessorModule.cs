using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Helpers;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Repository;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob
{
    using Configurations;

    public sealed class EventProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationProvider>()
                .As<IConfigurationProvider>()
                .SingleInstance();

            builder.RegisterType<MLDataProcessorHost>()
                .As<IMLDataProcessorHost>()
                .SingleInstance();

            builder.RegisterType<IotHubRepository>()
                .As<IIotHubRepository>();

            builder.RegisterType<VirtualDeviceTableStorage>()
                .As<IVirtualDeviceStorage>();
        }
    }
}
