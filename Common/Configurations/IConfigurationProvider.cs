// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Configurations
{
    public interface IConfigurationProvider
    {
        string GetConfigurationSettingValue(string configurationSettingName);

        string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue);
    }
}