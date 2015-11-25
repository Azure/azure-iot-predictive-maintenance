// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Web.Http;
    using System.Threading.Tasks;
    using Common.DeviceSchema;
    using Common.Repository;

    [Authorize]
    public sealed class SimulationController : System.Web.Http.ApiController
    {
        readonly IIotHubRepository iotHubRepository;

        public SimulationController(IIotHubRepository iotHubRepository)
        {
            this.iotHubRepository = iotHubRepository;
        }

        [HttpPost]
        [Route("api/simulation/start")]
        public async Task StartSimulation()
        {
            var command = CommandSchemaHelper.CreateNewCommand("StartTelemetry");

            await this.iotHubRepository.SendCommand("N1172FJ-1", command); //TODO: Pull IDs from service
            await this.iotHubRepository.SendCommand("N1172FJ-2", command);
        }

        [HttpPost]
        [Route("api/simulation/stop")]
        public async Task StopSimulation()
        {
            var command = CommandSchemaHelper.CreateNewCommand("StopTelemetry");

            await this.iotHubRepository.SendCommand("N1172FJ-1", command); //TODO: Pull IDs from service
            await this.iotHubRepository.SendCommand("N1172FJ-2", command);
        }
    }
}