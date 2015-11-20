using System;
using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    public class MLDataProcessorHost :
        Generic.EventProcessorHost<Generic.EventProcessorFactory<MLDataProcessor>>,
        IMLDataProcessorHost
    {
        public MLDataProcessorHost(
            IConfigurationProvider configurationProvider) :
            base(configurationProvider.GetConfigurationSettingValue("eventHub.HubName"),
                 configurationProvider.GetConfigurationSettingValue("eventHub.ConnectionString"),
                 configurationProvider.GetConfigurationSettingValue("eventHub.StorageConnectionString"),
                 configurationProvider)
        { }
    }
}
