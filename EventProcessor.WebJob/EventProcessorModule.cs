namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob
{
    using Autofac;
    using Common.Configurations;
    using Common.Execution;
    using Processors;

    public sealed class EventProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ShutdownFileWatcher>()
                .As<IShutdownFileWatcher>()
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