namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices.Factory
{
    using Common.Configurations;
    using Common.Models;
    using Logging;
    using Telemetry.Factory;
    using Transport.Factory;

    public interface IDeviceFactory
    {
        IDevice CreateDevice(ILogger logger, ITransportFactory transportFactory,
            ITelemetryFactory telemetryFactory, IConfigurationProvider configurationProvider, InitialDeviceConfig config);
    }
}