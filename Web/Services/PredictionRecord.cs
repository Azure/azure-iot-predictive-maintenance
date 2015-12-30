namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using WindowsAzure.Storage.Table;

    public sealed class PredictionRecord : TableEntity
    {
        public string TimeStamp { get; set; }

        public string Rul { get; set; }
    }
}