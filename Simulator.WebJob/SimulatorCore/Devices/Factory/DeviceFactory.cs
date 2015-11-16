using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Logging;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Telemetry.Factory;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport.Factory;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices.Factory
{
    using Configurations;

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
