namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models
{
    using WindowsAzure.Storage.Table;

    public class DeviceListEntity : TableEntity
    {
        public DeviceListEntity(string hostName, string deviceId)
        {
            this.PartitionKey = deviceId;
            this.RowKey = hostName;
        }

        public DeviceListEntity()
        {
        }

        [IgnoreProperty]
        public string HostName
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        [IgnoreProperty]
        public string DeviceId
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        public string Key { get; set; }
    }
}