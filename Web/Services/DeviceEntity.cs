namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using WindowsAzure.Storage.Table;

    public sealed class DeviceEntity : TableEntity
    {
        public string Key { get; set; }
    }
}