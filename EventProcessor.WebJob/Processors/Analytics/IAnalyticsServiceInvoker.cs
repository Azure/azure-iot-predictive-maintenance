namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    using System.Threading.Tasks;

    public interface IAnalyticsServiceInvoker
    {
        Task<string> GetRULAsync(string deviceId, string cycle, string sensor9, string sensor11, string sensor14, string sensor15);
    }
}
