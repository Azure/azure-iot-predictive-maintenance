namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models
{
    using WindowsAzure.Storage.Table;

    public class RulTableEntity : TableEntity
    {
        public string Rul { get; set; }
    }
}