namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Devices.Factory
{
    using Common.Configurations;
    using Common.Models;
    using SimulatorCore.Devices;
    using SimulatorCore.Devices.Factory;
    using SimulatorCore.Logging;
    using SimulatorCore.Telemetry.Factory;
    using SimulatorCore.Transport.Factory;

    public class EngineDeviceFactory : IDeviceFactory
    {
        public IDevice CreateDevice(ILogger logger, ITransportFactory transportFactory,
            ITelemetryFactory telemetryFactory, IConfigurationProvider configurationProvider, InitialDeviceConfig config)
        {
            var device = new EngineDevice(logger, transportFactory, telemetryFactory, configurationProvider);
            device.Init(config);
            return device;
        }
    }
}