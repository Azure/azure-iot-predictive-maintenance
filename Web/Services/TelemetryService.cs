// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using WindowsAzure.Storage.Table;
    using Common.Configurations;
    using Common.Helpers;
    using Contracts;

    public sealed class TelemetryService : ITelemetryService
    {
        readonly IConfigurationProvider configurationProvider;

        public TelemetryService(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;

        }

        public async Task<IEnumerable<Telemetry>> GetLatestData()
        {
            var storageConnectionString = this.configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString");

            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, "devicetelemetry");
            var tableQuery = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, DateTimeOffset.Now.AddHours(-24).DateTime);

            TableQuery<TelemetryRecord> query = new TableQuery<TelemetryRecord>()
                //.Where(tableQuery)
                .Take(100)
                .Select(new[] { "sensor11", "sensor14", "sensor15", "sensor9" });

            var telemetryData = new Collection<Telemetry>();

            // Print the fields for each customer.
            foreach (TelemetryRecord entity in table.ExecuteQuery(query))
            {
                var telemetry = new Telemetry
                {
                    DeviceId = entity.PartitionKey,
                    Timestamp = entity.Timestamp.DateTime,
                    Sensor1 = double.Parse(entity.sensor11),
                    Sensor2 = double.Parse(entity.sensor14),
                    Sensor3 = double.Parse(entity.sensor15),
                    Sensor4 = double.Parse(entity.sensor9)
                };

                telemetryData.Add(telemetry);
            }

            return telemetryData;
        }
    }
}