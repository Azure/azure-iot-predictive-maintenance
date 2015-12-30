namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Repository
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Configurations;
    using Common.Models;
    using Common.Repository;
    using Logging;
    using Properties;

    /// <summary>
    /// Sample repository that pulls the initial device config from the app.config file
    /// </summary>
    public class AppConfigRepository : IVirtualDeviceStorage
    {
        readonly string _hostName;
        readonly List<InitialDeviceConfig> _devices;
        readonly ILogger _logger;

        public AppConfigRepository(IConfigurationProvider configProvider, ILogger logger)
        {
            _devices = new List<InitialDeviceConfig>();
            _hostName = configProvider.GetConfigurationSettingValue("iotHub.HostName");
            _logger = logger;
        }

        public async Task<List<InitialDeviceConfig>> GetDeviceListAsync()
        {
            return await Task.Run(() =>
            {
                _logger.LogInfo("********** READING DEVICES FROM APP.CONFIG ********** ");
                if (_devices.Any())
                {
                    return _devices;
                }

                StringCollection deviceList = Settings.Default.DeviceList;

                foreach (string device in deviceList)
                {
                    string[] deviceConfigElements = device.Split(',');
                    var deviceConfig = new InitialDeviceConfig();

                    if (deviceConfigElements.Length > 1)
                    {
                        deviceConfig.DeviceId = deviceConfigElements[0];
                        deviceConfig.HostName = _hostName;
                        deviceConfig.Key = deviceConfigElements[1];

                        _devices.Add(deviceConfig);
                    }
                }

                return _devices;
            });
        }

        public Task<InitialDeviceConfig> GetDeviceAsync(string deviceId)
        {
            return Task.Run(() =>
            {
                if (!_devices.Any())
                {
                    return null;
                }

                return _devices.FirstOrDefault(x => x.DeviceId == deviceId);
            });
        }

        public Task AddOrUpdateDeviceAsync(InitialDeviceConfig deviceConfig)
        {
            return Task.Run(() =>
            {
                if (!_devices.Any())
                {
                    return;
                }

                var device = _devices.FirstOrDefault(x => x.DeviceId == deviceConfig.DeviceId);

                if (device != null)
                {
                    device.Key = deviceConfig.Key;
                    device.HostName = deviceConfig.HostName;
                }
                else
                {
                    _devices.Add(deviceConfig);
                }
            });
        }

        public Task<bool> RemoveDeviceAsync(string deviceId)
        {
            return Task.Run(() =>
            {
                if (!_devices.Any())
                {
                    return false;
                }

                var device = _devices.FirstOrDefault(x => x.DeviceId == deviceId);

                if (device != null)
                {
                    return _devices.Remove(device);
                }

                return false;
            });
        }
    }
}