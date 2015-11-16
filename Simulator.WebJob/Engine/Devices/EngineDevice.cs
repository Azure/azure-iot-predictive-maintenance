using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.CommandProcessors;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.CommandProcessors;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Logging;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Telemetry.Factory;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport.Factory;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Devices
{
    using Configurations;

    /// <summary>
    /// Implementation of a specific device type that extends the BaseDevice functionality
    /// </summary>
    public class EngineDevice : DeviceBase
    {

        public EngineDevice(ILogger logger, ITransportFactory transportFactory,
            ITelemetryFactory telemetryFactory, IConfigurationProvider configurationProvider)
            : base(logger, transportFactory, telemetryFactory, configurationProvider)
        {
        }

        /// <summary>
        /// Builds up the set of commands that are supported by this device
        /// </summary>
        protected override void InitCommandProcessors()
        {
            var pingDeviceProcessor = new PingDeviceProcessor(this);
            var startCommandProcessor = new StartCommandProcessor(this);
            var stopCommandProcessor = new StopCommandProcessor(this);

            pingDeviceProcessor.NextCommandProcessor = startCommandProcessor;
            startCommandProcessor.NextCommandProcessor = stopCommandProcessor;

            RootCommandProcessor = pingDeviceProcessor;
        }

        public void StartTelemetryData()
        {
            var PredictiveMaintenanceTelemetry = (PredictiveMaintenanceTelemetry)_telemetryController;
            PredictiveMaintenanceTelemetry.TelemetryActive = true;
            Logger.LogInfo("Device {0}: Telemetry has started", DeviceID);
        }

        public void StopTelemetryData()
        {
            var PredictiveMaintenanceTelemetry = (PredictiveMaintenanceTelemetry)_telemetryController;
            PredictiveMaintenanceTelemetry.TelemetryActive = false;
            Logger.LogInfo("Device {0}: Telemetry has stopped", DeviceID);
        }
    }
}
