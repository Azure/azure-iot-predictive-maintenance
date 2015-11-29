using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models.Commands;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Logging;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Telemetry;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry
{
    public class PredictiveMaintenanceTelemetry : ITelemetry
    {
        private readonly ILogger _logger;
        private readonly string _deviceId;
        private readonly IConfigurationProvider _config;

        private const int REPORT_FREQUENCY_IN_SECONDS = 1;

        private IEnumerator<IDictionary<string, object>> _data;
        private bool _active;

        public bool TelemetryActive
        {
            get { return _active; }
            set
            {
                // When we turn on telemetry, we reset our device data from the dataset
                if (!_active && value)
                {
                    _data.Reset();
                }
                _active = value;
                if (_active)
                {
                    StateTableEntity.Write(_deviceId, StartStopConstants.STARTED,
                        _config.GetConfigurationSettingValue("device.StorageConnectionString"),
                        _config.GetConfigurationSettingValue("SimulatorStateTableName")).Wait();
                }
                else
                {
                    StateTableEntity.Write(_deviceId, StartStopConstants.STOPPED,
                        _config.GetConfigurationSettingValue("device.StorageConnectionString"),
                        _config.GetConfigurationSettingValue("SimulatorStateTableName")).Wait();
                }
            }
        }

        public PredictiveMaintenanceTelemetry(IConfigurationProvider config, ILogger logger, string deviceId, IList<ExpandoObject> dataset)
        {
            _config = config;
            _logger = logger;
            _deviceId = deviceId;
            _active = false;
            _data = dataset.GetEnumerator();
        }

        public async Task SendEventsAsync(CancellationToken token, Func<object, Task> sendMessageAsync)
        {
            while (!token.IsCancellationRequested)
            {
                if (_active)
                {
                    try
                    {
                        // Search the data for the next row that contains this device ID
                        while (_data.MoveNext() && !_data.Current.Values.Contains(_deviceId)) ;

                        if (_data.Current != null)
                        {
                            _logger.LogInfo(_deviceId + " =>\n\t" + string.Join("\n\t", _data.Current.Select(m => m.Key + ": " + m.Value.ToString()).ToArray()));

                            await sendMessageAsync(_data.Current);
                        }
                        else
                        {
                            // End of the data; stop replaying
                            TelemetryActive = false;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // The data has been modified; stop replaying
                        TelemetryActive = false;
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(REPORT_FREQUENCY_IN_SECONDS), token);
            }
        }
    }
}