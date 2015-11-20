using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Execution;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob
{
    using Configurations;

    public sealed class EventProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ShutdownFileRunner>()
                .As<IShutdownFileRunner>()
                .SingleInstance();

            builder.RegisterType<ConfigurationProvider>()
                .As<IConfigurationProvider>()
                .SingleInstance();

            builder.RegisterType<MLDataProcessorHost>()
                .As<IMLDataProcessorHost>()
                .SingleInstance();
        }
    }
}
