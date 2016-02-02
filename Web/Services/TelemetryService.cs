namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using WindowsAzure.Storage.Table;
    using Common.Helpers;
    using Contracts;

    public sealed class TelemetryService : ITelemetryService
    {
        const int TimeOffsetInSeconds = 120;
        const int MaxRecordsToSend = 50;
        const int MaxRecordsToReceive = 200;

        readonly Settings _settings;

        public TelemetryService(Settings settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<Telemetry>> GetLatestTelemetry(string deviceId)
        {
            var storageConnectionString = _settings.StorageConnectionString;
            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, _settings.TelemetryTableName);
            var startTime = DateTimeOffset.Now.AddSeconds(-TimeOffsetInSeconds).DateTime;

            var deviceFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, deviceId);
            var timestampFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, startTime);
            var filter = TableQuery.CombineFilters(deviceFilter, TableOperators.And, timestampFilter);

            TableQuery<TelemetryEntity> query = new TableQuery<TelemetryEntity>()
                .Where(filter)
                .Take(MaxRecordsToReceive)
                .Select(new[] { "sensor11", "sensor14", "sensor15", "sensor9" });

            var result = new Collection<Telemetry>();
            var entities = table.ExecuteQuery(query)
                .OrderByDescending(x => x.Timestamp)
                .Take(MaxRecordsToSend);

            foreach (var entity in entities)
            {
                var telemetry = new Telemetry
                {
                    DeviceId = entity.PartitionKey,
                    RecordId = entity.RowKey,
                    Timestamp = entity.Timestamp.DateTime,
                    Sensor1 = Math.Round(double.Parse(entity.sensor11, CultureInfo.InvariantCulture)),
                    Sensor2 = Math.Round(double.Parse(entity.sensor14, CultureInfo.InvariantCulture)),
                    Sensor3 = Math.Round(double.Parse(entity.sensor15, CultureInfo.InvariantCulture)),
                    Sensor4 = Math.Round(double.Parse(entity.sensor9, CultureInfo.InvariantCulture))
                };
                result.Add(telemetry);
            }

            return result.OrderBy(x => x.Timestamp);
        }

        public async Task<IEnumerable<Prediction>> GetLatestPrediction(string deviceId)
        {
            var storageConnectionString = _settings.StorageConnectionString;
            var table = await AzureTableStorageHelper.GetTableAsync(storageConnectionString, _settings.PredictionTableName);
            var startTime = DateTimeOffset.Now.AddSeconds(-TimeOffsetInSeconds).DateTime;

            var deviceFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, deviceId);
            var timestampFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, startTime);
            var filter = TableQuery.CombineFilters(deviceFilter, TableOperators.And, timestampFilter);

            TableQuery<PredictionRecord> query = new TableQuery<PredictionRecord>()
                .Where(filter)
                .Take(MaxRecordsToReceive)
                .Select(new[] { "Timestamp", "Rul" });

            var result = new Collection<Prediction>();
            var entities = table.ExecuteQuery(query)
                .OrderByDescending(x => x.RowKey)
                .Take(MaxRecordsToSend);

            foreach (var entity in entities)
            {
                var prediction = new Prediction
                {
                    DeviceId = entity.PartitionKey,
                    Timestamp = entity.Timestamp.DateTime,
                    RemainingUsefulLife = (int)double.Parse(entity.Rul, CultureInfo.InvariantCulture),
                    Cycles = int.Parse(entity.RowKey, CultureInfo.InvariantCulture)
                };
            result.Add(prediction);
        }

            return result.OrderBy(x => x.Cycles);
        }
}
}