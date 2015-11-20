// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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

        public async Task<IEnumerable<Telemetry>> GetLatestTelemetryData()
        {
            var storageConnectionString = this.configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString");

            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, "devicetelemetry");
            //var tableQuery = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, DateTimeOffset.Now.AddHours(-24).DateTime);

            TableQuery<TelemetryRecord> query = new TableQuery<TelemetryRecord>()
                //.Where(tableQuery)
                .Take(2000)
                .Select(new[] { "sensor11", "sensor14", "sensor15", "sensor9" });

            var telemetryData = new Collection<Telemetry>();

            var entities = table.ExecuteQuery(query).OrderBy(x => x.Timestamp);

            // Print the fields for each customer.
            foreach (var entity in entities)
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


        public async Task<IEnumerable<Prediction>> GetLatestPredictionData()
        {
            var storageConnectionString = this.configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString");

            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, "devicemlresult");
            //var tableQuery = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, DateTimeOffset.Now.AddHours(-24).DateTime);

            TableQuery<PredictionRecord> query = new TableQuery<PredictionRecord>()
                //.Where(tableQuery)
                .Take(2000)
                .Select(new[] { "Timestamp", "Rul" });

            var predictionData = new Collection<Prediction>();

            var entities = table.ExecuteQuery(query).OrderBy(x => x.Timestamp);

            // Print the fields for each customer.
            foreach (var entity in entities)
            {
                var prediction = new Prediction
                {
                    DeviceId = entity.PartitionKey,
                    Timestamp = entity.Timestamp.DateTime,
                    RemainingUsefulLife = (int)double.Parse(entity.Rul),
                    Cycles = int.Parse(entity.RowKey)
                };

                predictionData.Add(prediction);
            }

            return predictionData;
        }
    }
}