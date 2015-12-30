namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Configurations;
    using Common.Models;
    using Common.Models.Commands;
    using SimulatorCore.Logging;
    using SimulatorCore.Telemetry;

    public class PredictiveMaintenanceTelemetry : ITelemetry
    {
        readonly ILogger _logger;
        readonly string _deviceId;
        readonly IConfigurationProvider _config;

        const int REPORT_FREQUENCY_IN_SECONDS = 1;

        readonly IEnumerator<IDictionary<string, object>> _data;
        bool _active;

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
                        while (_data.MoveNext() && !_data.Current.Values.Contains(_deviceId)) { }

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