namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    using Common.Configurations;
    using Generic;

    public class MLDataProcessorHost :
        EventProcessorHost<EventProcessorFactory<MLDataProcessor>>,
        IMLDataProcessorHost
    {
        public MLDataProcessorHost(
            IConfigurationProvider configurationProvider)
            :
                base(configurationProvider.GetConfigurationSettingValue("eventHub.HubName"),
                    configurationProvider.GetConfigurationSettingValue("eventHub.ConnectionString"),
                    configurationProvider.GetConfigurationSettingValue("eventHub.StorageConnectionString"),
                    configurationProvider)
        {
        }
    }
}