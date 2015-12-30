namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models
{
    using System.Threading.Tasks;
    using WindowsAzure.Storage.Table;
    using Helpers;

    public class StateTableEntity : TableEntity
    {
        public string State { get; set; }

        public static async Task Write(string device, string state, string connectionString, string table)
        {
            StateTableEntity entry = new StateTableEntity
            {
                PartitionKey = device,
                RowKey = "State", // Arbitrary constant; we're only storing one value per device
                State = state,
                ETag = "*"
            };

            // We don't need a data model to represent the result of this operation,
            // so we use a stub table/model convertor
            await AzureTableStorageHelper.DoTableInsertOrReplaceAsync<object, StateTableEntity>(entry, (StateTableEntity e) => { return null; }, connectionString, table);
        }
    }
}