namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using Common.Configurations;

    public sealed class Settings
    {
        readonly IConfigurationProvider _configurationProvider;

        public Settings(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string TelemetryTableName
        {
            get { return _configurationProvider.GetConfigurationSettingValue("TelemetryStoreContainerName"); }
        }

        public string PredictionTableName
        {
            get { return _configurationProvider.GetConfigurationSettingValue("MLResultTableName"); }
        }

        public string StorageConnectionString
        {
            get { return _configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString"); }
        }

        public string SimulatorStateTableName
        {
            get { return _configurationProvider.GetConfigurationSettingValue("SimulatorStateTableName"); }
        }
    }
}