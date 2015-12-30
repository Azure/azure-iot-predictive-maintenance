namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using WindowsAzure.ServiceRuntime;

    public class ConfigurationProvider : IConfigurationProvider, IDisposable
    {
        readonly Dictionary<string, string> _configuration = new Dictionary<string, string>();
        EnvironmentDescription _environment;
        const string ConfigToken = "config:";
        bool _disposed;

        public string GetConfigurationSettingValue(string configurationSettingName)
        {
            return GetConfigurationSettingValueOrDefault(configurationSettingName, string.Empty);
        }

        public string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue)
        {
            try
            {
                if (!_configuration.ContainsKey(configurationSettingName))
                {
                    string configValue = string.Empty;
                    bool isEmulated = true;
                    bool isAvailable = false;
                    try
                    {
                        isAvailable = RoleEnvironment.IsAvailable;
                    }
                    catch (TypeInitializationException)
                    {
                    }
                    if (isAvailable)
                    {
                        configValue = RoleEnvironment.GetConfigurationSettingValue(configurationSettingName);
                        isEmulated = RoleEnvironment.IsEmulated;
                    }
                    else
                    {
                        configValue = ConfigurationManager.AppSettings[configurationSettingName];
                        isEmulated = Environment.CommandLine.Contains("iisexpress.exe") ||
                            Environment.CommandLine.Contains("WebJob.vshost.exe");
                    }
                    if (isEmulated && (configValue != null && configValue.StartsWith(ConfigToken, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (_environment == null)
                        {
                            LoadEnvironmentConfig();
                        }

                        configValue =
                            _environment.GetSetting(configValue.Substring(configValue.IndexOf(ConfigToken, StringComparison.Ordinal) + ConfigToken.Length));
                    }
                    try
                    {
                        _configuration.Add(configurationSettingName, configValue);
                    }
                    catch (ArgumentException)
                    {
                        // at this point, this key has already been added on a different
                        // thread, so we're fine to continue
                    }
                }
            }
            catch (RoleEnvironmentException)
            {
                if (string.IsNullOrEmpty(defaultValue))
                {
                    throw;
                }

                _configuration.Add(configurationSettingName, defaultValue);
            }
            return _configuration[configurationSettingName];
        }

        void LoadEnvironmentConfig()
        {
            var executingPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            // Check for build_output
            int buildLocation = executingPath.IndexOf("Build_Output", StringComparison.OrdinalIgnoreCase);
            if (buildLocation >= 0)
            {
                string fileName = executingPath.Substring(0, buildLocation) + "local.config.user";
                if (File.Exists(fileName))
                {
                    _environment = new EnvironmentDescription(fileName);
                    return;
                }
            }

            // Web roles run in there app dir so look relative
            int location = executingPath.IndexOf("Web\\bin", StringComparison.OrdinalIgnoreCase);

            if (location == -1)
            {
                location = executingPath.IndexOf("WebJob\\bin", StringComparison.OrdinalIgnoreCase);
            }
            if (location >= 0)
            {
                string fileName = executingPath.Substring(0, location) + "local.config.user";
                if (File.Exists(fileName))
                {
                    _environment = new EnvironmentDescription(fileName);
                    return;
                }
            }

            throw new ArgumentException("Unable to locate local.config.user file.  Make sure you have run 'build.cmd local'.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_environment != null)
                {
                    _environment.Dispose();
                }
            }

            _disposed = true;
        }

        ~ConfigurationProvider()
        {
            Dispose(false);
        }
    }
}