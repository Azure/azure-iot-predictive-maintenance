using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Helpers;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Logging;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Telemetry.Factory;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Telemetry.Factory
{
    public class EngineTelemetryFactory : ITelemetryFactory
    {
        private readonly ILogger _logger;

        private IList<ExpandoObject> _dataset;

        public EngineTelemetryFactory(ILogger logger, IConfigurationProvider config)
        {
            _logger = logger;

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

            var monitorTelemetry = new PredictiveMaintenanceTelemetry(_logger, device.DeviceID, _dataset);
            device.TelemetryEvents.Add(monitorTelemetry);

            return monitorTelemetry;
        }
    }
}
