namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry.Factory
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using WindowsAzure.Storage;
    using Common.Configurations;
    using Common.Helpers;
    using SimulatorCore.Devices;
    using SimulatorCore.Logging;
    using SimulatorCore.Telemetry.Factory;

    public class EngineTelemetryFactory : ITelemetryFactory
    {
        readonly ILogger _logger;
        readonly IConfigurationProvider _config;

        readonly IList<ExpandoObject> _dataset;

        public EngineTelemetryFactory(ILogger logger, IConfigurationProvider config)
        {
            _logger = logger;
            _config = config;

            // This will load the CSV data from the specified file in blob storage;
            // any failure in accessing or reading the data will be handled as an exception
            Stream dataStream = CloudStorageAccount
                .Parse(config.GetConfigurationSettingValue("device.StorageConnectionString"))
                .CreateCloudBlobClient()
                .GetContainerReference(config.GetConfigurationSettingValue("SimulatorDataContainer"))
                .GetBlockBlobReference(config.GetConfigurationSettingValue("SimulatorDataFileName"))
                .OpenRead();

            _dataset = ParsingHelper.ParseCsv(new StreamReader(dataStream)).ToExpandoObjects().ToList();
        }

        public object PopulateDeviceWithTelemetryEvents(IDevice device)
        {
            var startupTelemetry = new StartupTelemetry(_logger, device);
            device.TelemetryEvents.Add(startupTelemetry);

            var monitorTelemetry = new PredictiveMaintenanceTelemetry(_config, _logger, device.DeviceID, _dataset);
            device.TelemetryEvents.Add(monitorTelemetry);

            return monitorTelemetry;
        }
    }
}