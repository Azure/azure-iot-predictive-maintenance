namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SimulatorCore.Devices;
    using SimulatorCore.Logging;
    using SimulatorCore.Telemetry;

    public class StartupTelemetry : ITelemetry
    {
        readonly ILogger _logger;
        readonly IDevice _device;

        public StartupTelemetry(ILogger logger, IDevice device)
        {
            _logger = logger;
            _device = device;
        }

        public async Task SendEventsAsync(CancellationToken token, Func<object, Task> sendMessageAsync)
        {
            if (!token.IsCancellationRequested)
            {
                _logger.LogInfo("Sending initial data for device {0}", _device.DeviceID);
                await sendMessageAsync(_device.GetDeviceInfo());
            }
        }
    }
}