using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models
{
    public class StateTableEntity : TableEntity
    {
        public string State { get; set; }

        public static async Task Write(string device, string state, string connectionString, string table)
        {
            StateTableEntity entry = new StateTableEntity()
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
