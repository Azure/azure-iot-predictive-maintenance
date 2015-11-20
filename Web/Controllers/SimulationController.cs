// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Web.Http;
    using Common.DeviceSchema;
    using Common.Repository;

    [Authorize]
    public sealed class SimulationController : System.Web.Http.ApiController
    {
        private readonly IIotHubRepository iotHubRepository;

        public SimulationController(IIotHubRepository iotHubRepository)
        {
            this.iotHubRepository = iotHubRepository;
        }

        [HttpPost]
        [Route("api/simulation/start")]
        public void StartSimulation()
        {
            var command = CommandSchemaHelper.CreateNewCommand("StartTelemetry");

            this.iotHubRepository.SendCommand("N1172FJ-1", command); //TODO: Pull IDs from service
            this.iotHubRepository.SendCommand("N1172FJ-2", command);
        }

        [HttpPost]
        [Route("api/simulation/stop")]
        public void StopSimulation()
        {
            var command = CommandSchemaHelper.CreateNewCommand("StopTelemetry");

            this.iotHubRepository.SendCommand("N1172FJ-1", command); //TODO: Pull IDs from service
            this.iotHubRepository.SendCommand("N1172FJ-2", command);
        }
    }
}