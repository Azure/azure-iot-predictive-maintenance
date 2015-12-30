namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport.Factory
{
    using Common.Configurations;
    using Devices;
    using Logging;
    using Serialization;

    public class IotHubTransportFactory : ITransportFactory
    {
        readonly ISerialize _serializer;
        readonly ILogger _logger;
        readonly IConfigurationProvider _configurationProvider;

        public IotHubTransportFactory(ISerialize serializer, ILogger logger,
            IConfigurationProvider configurationProvider)
        {
            _serializer = serializer;
            _logger = logger;
            _configurationProvider = configurationProvider;
        }

        public ITransport CreateTransport(IDevice device)
        {
            return new IoTHubTransport(_serializer, _logger, _configurationProvider, device);
        }
    }
}