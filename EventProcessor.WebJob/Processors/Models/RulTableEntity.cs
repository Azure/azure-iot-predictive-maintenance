using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models
{
    public class RulTableEntity : TableEntity
    {
        public string Rul { get; set; }
    }
}
