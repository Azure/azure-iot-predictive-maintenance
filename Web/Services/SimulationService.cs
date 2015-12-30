namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WindowsAzure.Storage.Table;
    using Common.DeviceSchema;
    using Common.Helpers;
    using Common.Models;
    using Common.Models.Commands;
    using Common.Repository;

    public sealed class SimulationService : ISimulationService
    {
        readonly IDeviceService _deviceService;
        readonly IIotHubRepository _iotHubRepository;
        readonly string _storageConnectionString;
        readonly string _telemetryTableName;
        readonly string _mlResultTableName;
        readonly string _simulatorStateTableName;

        public SimulationService(IDeviceService deviceService, IIotHubRepository iotHubRepository, Settings settings)
        {
            _deviceService = deviceService;
            _iotHubRepository = iotHubRepository;
            _storageConnectionString = settings.StorageConnectionString;
            _telemetryTableName = settings.TelemetryTableName;
            _mlResultTableName = settings.PredictionTableName;
            _simulatorStateTableName = settings.SimulatorStateTableName;
        }

        public async Task<string> StartSimulation()
        {
            ClearTables();

            await WriteState(StartStopConstants.STARTING);
            await SendCommand("StartTelemetry");

            return StartStopConstants.STARTING;
        }

        public async Task<string> StopSimulation()
        {
            await WriteState(StartStopConstants.STOPPING);
            await SendCommand("StopTelemetry");

            return StartStopConstants.STOPPING;
        }

        public async Task<string> GetSimulationState()
        {
            var table = await AzureTableStorageHelper.GetTableAsync(_storageConnectionString, _simulatorStateTableName);
            var query = new TableQuery<StateTableEntity>()
                .Take(1)
                .Select(new[] { "State" });

            var result = table.ExecuteQuery(query);
            var stateEntity = result.FirstOrDefault();

            return stateEntity != null ? stateEntity.State : StartStopConstants.STOPPED;
        }

        async void ClearTables()
        {
            var telemetryTable = await AzureTableStorageHelper.GetTableAsync(_storageConnectionString, _telemetryTableName);
            var mlTable = await AzureTableStorageHelper.GetTableAsync(_storageConnectionString, _mlResultTableName);

            ClearTable(telemetryTable);
            ClearTable(mlTable);
        }

        void ClearTable(CloudTable table)
        {
            var deviceIds = _deviceService.GetDeviceIds();

            foreach (var partitionKey in deviceIds)
            {
                TableBatchOperation batchDelete = new TableBatchOperation();

                // gets all the entities in the table for this partition key
                string partitionCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
                List<DynamicTableEntity> entities = table.ExecuteQuery(new TableQuery().Where(partitionCondition)).ToList();

                entities.ForEach(e =>
                {
                    batchDelete.Add(TableOperation.Delete(e));

                    // Azure has a limit on batch operations
                    if (batchDelete.Count == 100)
                    {
                        table.ExecuteBatch(batchDelete);
                        batchDelete = new TableBatchOperation();
                    }
                });

                // flush out whatever is left
                if (batchDelete.Count > 0)
                {
                    table.ExecuteBatch(batchDelete);
                }
            }
        }

        async Task SendCommand(string commandName)
        {
            var command = CommandSchemaHelper.CreateNewCommand(commandName);

            foreach (var partitionKey in _deviceService.GetDeviceIds())
            {
                await _iotHubRepository.SendCommand(partitionKey, command);
            }
        }

        async Task WriteState(string state)
        {
            foreach (var partitionKey in _deviceService.GetDeviceIds())
            {
                await StateTableEntity.Write(partitionKey, state, _storageConnectionString, _simulatorStateTableName);
            }
        }
    }
}