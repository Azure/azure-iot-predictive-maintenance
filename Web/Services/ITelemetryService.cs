namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;

    public interface ITelemetryService
    {
        Task<IEnumerable<Prediction>> GetLatestPrediction(string deviceId);

        Task<IEnumerable<Telemetry>> GetLatestTelemetry(string deviceId);
    }
}