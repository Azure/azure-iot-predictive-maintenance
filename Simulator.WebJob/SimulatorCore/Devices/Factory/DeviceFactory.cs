namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices.Factory
{
    using Common.Configurations;
    using Common.Models;
    using Logging;
    using Telemetry.Factory;
    using Transport.Factory;

    public class DeviceFactory : IDeviceFactory
    {
        public IDevice CreateDevice(ILogger logger, ITransportFactory transportFactory,
            ITelemetryFactory telemetryFactory, IConfigurationProvider configurationProvider, InitialDeviceConfig config)
        {
            var device = new DeviceBase(logger, transportFactory, telemetryFactory, configurationProvider);
            device.Init(config);
            return device;
        }
    }
}