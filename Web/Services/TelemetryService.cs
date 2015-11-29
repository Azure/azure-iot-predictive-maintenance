// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System;
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
        const int TimeOffsetInSeconds = 30;
        readonly IConfigurationProvider configurationProvider;

        public TelemetryService(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public async Task<IEnumerable<Telemetry>> GetLatestTelemetry(string deviceId)
        {
            var storageConnectionString = this.configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString");
            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, "devicetelemetry");
            var dateTime = DateTimeOffset.Now.AddSeconds(-TimeOffsetInSeconds).DateTime;

            TableQuery<TelemetryEntity> query = new TableQuery<TelemetryEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, deviceId))
                .Where(TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, dateTime))
                .Take(TimeOffsetInSeconds)
                .Select(new[] { "sensor11", "sensor14", "sensor15", "sensor9" });

            var result = new Collection<Telemetry>();
            var entities = table.ExecuteQuery(query).OrderBy(x => x.Timestamp);

            foreach (var entity in entities)
            {
                var telemetry = new Telemetry
                {
                    DeviceId = entity.PartitionKey,
                    RecordId = entity.RowKey,
                    Timestamp = entity.Timestamp.DateTime,
                    Sensor1 = Math.Round(double.Parse(entity.sensor11)),
                    Sensor2 = Math.Round(double.Parse(entity.sensor14)),
                    Sensor3 = Math.Round(double.Parse(entity.sensor15)),
                    Sensor4 = Math.Round(double.Parse(entity.sensor9))
                };
                result.Add(telemetry);
            }

            return result;
        }

        public async Task<IEnumerable<Prediction>> GetLatestPrediction(string deviceId)
        {
            var storageConnectionString = this.configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString");
            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, "devicemlresult");

            var dateTime = DateTimeOffset.Now.AddSeconds(-TimeOffsetInSeconds).DateTime;

            TableQuery<PredictionRecord> query = new TableQuery<PredictionRecord>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, deviceId))
                .Where(TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, dateTime))
                .Take(TimeOffsetInSeconds)
                .Select(new[] { "Timestamp", "Rul" });

            var result = new Collection<Prediction>();
            var entities = table.ExecuteQuery(query).OrderBy(x => x.Timestamp);

            foreach (var entity in entities)
            {
                var prediction = new Prediction
                {
                    DeviceId = entity.PartitionKey,
                    Timestamp = entity.Timestamp.DateTime,
                    RemainingUsefulLife = (int)double.Parse(entity.Rul),
                    Cycles = int.Parse(entity.RowKey)
                };
                result.Add(prediction);
            }

            return result;
        }
    }
}