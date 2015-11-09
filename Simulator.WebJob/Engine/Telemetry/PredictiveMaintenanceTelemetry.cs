using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Logging;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Telemetry;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry
{
    public class PredictiveMaintenanceTelemetry : ITelemetry
    {
        private readonly ILogger _logger;
        private readonly string _deviceId;

        private const int REPORT_FREQUENCY_IN_SECONDS = 5;

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
            }
        }

        public PredictiveMaintenanceTelemetry(ILogger logger, string deviceId, IList<ExpandoObject> dataset)
        {
            _logger = logger;
            _deviceId = deviceId;
            _active = false;
            _data = dataset.GetEnumerator();

            TelemetryActive = true;
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

                        _logger.LogInfo(_deviceId + " =>\n\t" + string.Join("\n\t", _data.Current.Select(m => m.Key + ": " + m.Value.ToString()).ToArray()));

                        await sendMessageAsync(_data.Current);
                    }
                    catch (InvalidOperationException)
                    {
                        // End of the data or the data has been modified; stop replaying
                        TelemetryActive = false;
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(REPORT_FREQUENCY_IN_SECONDS), token);
            }
        }
    }
}