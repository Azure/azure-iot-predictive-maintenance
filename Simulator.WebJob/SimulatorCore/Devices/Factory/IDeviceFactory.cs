using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Telemetry.Factory;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport.Factory;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices.Factory
{
    public interface IDeviceFactory
    {
        IDevice CreateDevice(Logging.ILogger logger, ITransportFactory transportFactory,
            ITelemetryFactory telemetryFactory, IConfigurationProvider configurationProvider, InitialDeviceConfig config);
    }
}
