// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Web.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Common.DeviceSchema;
    using Common.Repository;
    using Common.Configurations;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    [Authorize]
    public sealed class SimulationController : System.Web.Http.ApiController
    {
        readonly IIotHubRepository iotHubRepository;
        readonly string storageConnectionString;
        readonly string telemetryTableName;
        readonly string mlResultTableName;

        public SimulationController(IIotHubRepository iotHubRepository, IConfigurationProvider configProvider)
        {
            this.iotHubRepository = iotHubRepository;
            this.storageConnectionString = configProvider.GetConfigurationSettingValue("device.StorageConnectionString");
            this.telemetryTableName = configProvider.GetConfigurationSettingValue("TelemetryStoreContainerName");
            this.mlResultTableName = configProvider.GetConfigurationSettingValue("MLResultTableName");
        }

        [HttpPost]
        [Route("api/simulation/start")]
        public async Task StartSimulation()
        {
            this.clearTables();
            await sendCommand("StartTelemetry");
        }

        [HttpPost]
        [Route("api/simulation/stop")]
        public async Task StopSimulation()
        {
            await sendCommand("StopTelemetry");
        }

        private string[] partitionKeys()
        {
            return new string[]
            {
                "N1172FJ-1",
                "N1172FJ-2"
            };
        }

        private void clearTables()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable telemetryTable = tableClient.GetTableReference(this.telemetryTableName);
            CloudTable mlTable = tableClient.GetTableReference(this.mlResultTableName);

            clearTable(telemetryTable);
            clearTable(mlTable);
        }

        private void clearTable(CloudTable table)
        {
            string[] partitionKeys = this.partitionKeys();
            TableBatchOperation batchDelete = new TableBatchOperation();

            //gets all the entities in the table
            List<DynamicTableEntity> entities = table.ExecuteQuery(new TableQuery()).ToList();

            entities.ForEach(e =>
            {
                if (partitionKeys.Contains(e.PartitionKey))
                {
                    batchDelete.Add(TableOperation.Delete(e));

                    //Azure has a limit on batch operations
                    if(batchDelete.Count == 100)
                    {
                        table.ExecuteBatch(batchDelete);
                        batchDelete = new TableBatchOperation();
                    }
                }
            });

            //flush out whatever is left
            table.ExecuteBatch(batchDelete);
        }

        private async Task sendCommand(string commandName)
        {
            var command = CommandSchemaHelper.CreateNewCommand(commandName);

            foreach (var partitionKey in this.partitionKeys())
            {
                await this.iotHubRepository.SendCommand(partitionKey, command);
            }
        }
    }
}