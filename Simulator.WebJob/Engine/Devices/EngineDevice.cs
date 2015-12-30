namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Devices
{
    using CommandProcessors;
    using Common.Configurations;
    using SimulatorCore.CommandProcessors;
    using SimulatorCore.Devices;
    using SimulatorCore.Logging;
    using SimulatorCore.Telemetry.Factory;
    using SimulatorCore.Transport.Factory;
    using Telemetry;

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
            var predictiveMaintenanceTelemetry = (PredictiveMaintenanceTelemetry)TelemetryController;
            predictiveMaintenanceTelemetry.TelemetryActive = true;
            Logger.LogInfo("Device {0}: Telemetry has started", DeviceID);
        }

        public void StopTelemetryData()
        {
            var predictiveMaintenanceTelemetry = (PredictiveMaintenanceTelemetry)TelemetryController;
            predictiveMaintenanceTelemetry.TelemetryActive = false;
            Logger.LogInfo("Device {0}: Telemetry has stopped", DeviceID);
        }
    }
}