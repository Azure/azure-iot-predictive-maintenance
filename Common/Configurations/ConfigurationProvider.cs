// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using WindowsAzure.ServiceRuntime;
    using Common.Configurations;

    public class ConfigurationProvider : IConfigurationProvider, IDisposable
    {
        readonly Dictionary<string, string> configuration = new Dictionary<string, string>();
        EnvironmentDescription environment;
        const string ConfigToken = "config:";
        bool _disposed;

        public string GetConfigurationSettingValue(string configurationSettingName)
        {
            return this.GetConfigurationSettingValueOrDefault(configurationSettingName, string.Empty);
        }

        public string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue)
        {
            try
            {
                if (!this.configuration.ContainsKey(configurationSettingName))
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
                        if (this.environment == null)
                        {
                            this.LoadEnvironmentConfig();
                        }

                        configValue =
                            this.environment.GetSetting(configValue.Substring(configValue.IndexOf(ConfigToken, StringComparison.Ordinal) + ConfigToken.Length));
                    }
                    try
                    {
                        this.configuration.Add(configurationSettingName, configValue);
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

                this.configuration.Add(configurationSettingName, defaultValue);
            }
            return this.configuration[configurationSettingName];
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
                    this.environment = new EnvironmentDescription(fileName);
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
                    this.environment = new EnvironmentDescription(fileName);
                    return;
                }
            }

            throw new ArgumentException("Unable to locate local.config.user file.  Make sure you have run 'build.cmd local'.");
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.environment != null)
                {
                    this.environment.Dispose();
                }
            }

            this._disposed = true;
        }

        ~ConfigurationProvider()
        {
            this.Dispose(false);
        }
    }
}