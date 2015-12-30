namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Services;

    [Authorize]
    public sealed class SimulationController : ApiController
    {
        readonly ISimulationService _simulationService;

        public SimulationController(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        [HttpPost]
        [Route("api/simulation/start")]
        public async Task<string> StartSimulation()
        {
            return await _simulationService.StartSimulation();
        }

        [HttpPost]
        [Route("api/simulation/stop")]
        public async Task<string> StopSimulation()
        {
            return await _simulationService.StopSimulation();
        }

        [HttpGet]
        [Route("api/simulation/state")]
        public async Task<string> GetSimulationState()
        {
            return await _simulationService.GetSimulationState();
        }
    }
}