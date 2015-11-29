// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using WindowsAzure.Storage;
    using WindowsAzure.Storage.Table;
    using Common.Configurations;
    using Common.DeviceSchema;
    using Common.Repository;
    using Common.Models;
    using Common.Models.Commands;

    [Authorize]
    public sealed class SimulationController : ApiController
    {
        readonly IIotHubRepository iotHubRepository;
        readonly string storageConnectionString;
        readonly string telemetryTableName;
        readonly string mlResultTableName;
        readonly string simulatorStateTableName;

        public SimulationController(IIotHubRepository iotHubRepository, IConfigurationProvider configProvider)
        {
            this.iotHubRepository = iotHubRepository;
            this.storageConnectionString = configProvider.GetConfigurationSettingValue("device.StorageConnectionString");
            this.telemetryTableName = configProvider.GetConfigurationSettingValue("TelemetryStoreContainerName");
            this.mlResultTableName = configProvider.GetConfigurationSettingValue("MLResultTableName");
            this.simulatorStateTableName = configProvider.GetConfigurationSettingValue("SimulatorStateTableName");
        }

        [HttpPost]
        [Route("api/simulation/start")]
        public async Task StartSimulation()
        {

            this.ClearTables();
            await this.WriteState(StartStopConstants.STARTING);
            await this.SendCommand("StartTelemetry");
        }

        [HttpPost]
        [Route("api/simulation/stop")]
        public async Task StopSimulation()
        {
            await this.WriteState(StartStopConstants.STOPPING);
            await this.SendCommand("StopTelemetry");
        }

        string[] PartitionKeys()
        {
            return new[]
            {
                "N1172FJ-1",
                "N1172FJ-2"
            };
        }

        void ClearTables()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable telemetryTable = tableClient.GetTableReference(this.telemetryTableName);
            CloudTable mlTable = tableClient.GetTableReference(this.mlResultTableName);

            this.ClearTable(telemetryTable);
            this.ClearTable(mlTable);
        }

        void ClearTable(CloudTable table)
        {
            foreach (var partitionKey in this.PartitionKeys())
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

            foreach (var partitionKey in this.PartitionKeys())
            {
                await this.iotHubRepository.SendCommand(partitionKey, command);
            }
        }

        private async Task WriteState(string state)
        {
            foreach (var partitionKey in this.PartitionKeys())
            {
                await StateTableEntity.Write(partitionKey, state, this.storageConnectionString, this.simulatorStateTableName);
            }
        }
    }
}