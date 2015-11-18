using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    public class MLDataProcessorHost :
        Generic.EventProcessorHost<Generic.EventProcessorFactory<MLDataProcessor>>,
        IMLDataProcessorHost
    {
        public MLDataProcessorHost(
            ILifetimeScope scope) :
            this(scope.Resolve<IConfigurationProvider>())
        { }

        private MLDataProcessorHost(
            IConfigurationProvider configurationProvider) :
            base(configurationProvider.GetConfigurationSettingValue("eventHub.HubName"),
                 configurationProvider.GetConfigurationSettingValue("eventHub.ConnectionString"),
                 configurationProvider.GetConfigurationSettingValue("eventHub.StorageConnectionString"),
                  configurationProvider)
        { }
    }
}
