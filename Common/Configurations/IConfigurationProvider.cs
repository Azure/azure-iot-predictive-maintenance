namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations
{
    public interface IConfigurationProvider
    {
        string GetConfigurationSettingValue(string configurationSettingName);

        string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue);
    }
}