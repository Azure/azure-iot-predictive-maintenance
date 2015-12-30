namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using WindowsAzure.Storage.Table;

    public sealed class TelemetryEntity : TableEntity
    {
        public string sensor11 { get; set; }

        public string sensor14 { get; set; }

        public string sensor15 { get; set; }

        public string sensor9 { get; set; }
    }
}