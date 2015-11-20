// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Contracts;
    using Newtonsoft.Json;
    using Services;

    [Authorize]
    public sealed class DataController : System.Web.Http.ApiController
    {
        private readonly ITelemetryService telemetryService;

        public DataController(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService;
        }

        [HttpGet]
        [Route("api/devices")]
        public async Task<IEnumerable<Device>> Devices()
        {
            var devices = new Collection<Device>();

            devices.Add(new Device
            {
                Id = "N1172FJ-1",
                Status = "Running",
                Manufacturer = "Microsoft",
                Firmware = "1.92",
                Memory = "10 MB",
                ModelNumber = "MD-868",
                Platform = "Plat-80",
                Processor = "i3-5197",
                SerialNumber = "SER2049",
                Hostname = "IotSuiteLocalcdd50.azure-devices.net",
                CreatedTime = DateTime.Now.ToString("d"),
                UpdatedTime = DateTime.Now.ToString("d"),
                State = "Normal",
                HubEnabledState = true
            });

            devices.Add(new Device
            {
                Id = "N1172FJ-2",
                Status = "Pending",
                Manufacturer = "Contoso Inc.",
                Firmware = "1.91",
                Memory = "7 MB",
                ModelNumber = "MD-867",
                Platform = "Plat-79",
                Processor = "i3-5192",
                SerialNumber = "SER2048",
                Hostname = "IotSuiteLocalcdd50.azure-devices.net",
                CreatedTime = DateTime.Now.ToString("d"),
                UpdatedTime = DateTime.Now.ToString("d"),
                State = "Normal",
                HubEnabledState = true
            });

            return devices;
        }

        [HttpGet]
        [Route("api/telemetry")]
        public async Task<EnginesTelemetry> GetEnginesTelemetry()
        {
            var source = await this.telemetryService.GetLatestTelemetryData();

            var d = new Dictionary<string, Collection<Telemetry>>();


            foreach (var telemetry in source) //TODO: Make normal grouping by device ID
            {
                Collection<Telemetry> collection;

                if (d.ContainsKey(telemetry.DeviceId))
                {
                    collection = d[telemetry.DeviceId];
                }
                else
                {
                    collection = new Collection<Telemetry>();
                    d[telemetry.DeviceId] = collection;
                }

                collection.Add(telemetry);
            }

            var enginesTelemetry = new EnginesTelemetry
            {
                Engine1Telemetry = d.ElementAt(0).Value,
                Engine2Telemetry = d.ElementAt(1).Value
            };

            return enginesTelemetry;
        }

        [HttpGet]
        [Route("api/prediction")]
        public async Task<EnginesPrediction> GetEnginesPrediction()
        {

            var source = await this.telemetryService.GetLatestPredictionData();

            var d = new Dictionary<string, Collection<Prediction>>();


            foreach (var telemetry in source) //TODO: Make normal grouping by device ID
            {
                Collection<Prediction> collection;

                if (d.ContainsKey(telemetry.DeviceId))
                {
                    collection = d[telemetry.DeviceId];
                }
                else
                {
                    collection = new Collection<Prediction>();
                    d[telemetry.DeviceId] = collection;
                }

                collection.Add(telemetry);
            }

            var enginesTelemetry = new EnginesPrediction
            {
                Engine1Prediction = d.ElementAt(0).Value,
                Engine2Prediction = d.ElementAt(1).Value
            };

            return enginesTelemetry;
        }
    }
}