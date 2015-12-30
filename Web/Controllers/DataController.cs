namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Contracts;
    using Services;

    [Authorize]
    public sealed class DataController : ApiController
    {
        const string Engine1DeviceId = "N2172FJ-1";
        const string Engine2DeviceId = "N2172FJ-2";

        readonly ITelemetryService _telemetryService;

        public DataController(ITelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpGet]
        [Route("api/telemetry")]
        public async Task<EnginesTelemetry> GetEnginesTelemetry()
        {
            var engine1Telemetry = await _telemetryService.GetLatestTelemetry(Engine1DeviceId);
            var engine2Telemetry = await _telemetryService.GetLatestTelemetry(Engine2DeviceId);

            var enginesTelemetry = new EnginesTelemetry();
            enginesTelemetry.Engine1Telemetry = engine1Telemetry;
            enginesTelemetry.Engine2Telemetry = engine2Telemetry;

            return enginesTelemetry;
        }

        [HttpGet]
        [Route("api/prediction")]
        public async Task<EnginesPrediction> GetEnginesPrediction()
        {
            var engine1Prediction = await _telemetryService.GetLatestPrediction(Engine1DeviceId);
            var engine2Prediction = await _telemetryService.GetLatestPrediction(Engine2DeviceId);

            var enginesPrediction = new EnginesPrediction();
            enginesPrediction.Engine1Prediction = engine1Prediction;
            enginesPrediction.Engine2Prediction = engine2Prediction;

            return enginesPrediction;
        }
    }
}