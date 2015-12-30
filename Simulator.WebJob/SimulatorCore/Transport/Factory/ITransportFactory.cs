namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport.Factory
{
    using Devices;

    public interface ITransportFactory
    {
        ITransport CreateTransport(IDevice device);
    }
}