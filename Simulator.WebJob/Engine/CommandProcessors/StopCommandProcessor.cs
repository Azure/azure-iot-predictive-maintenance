using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.Devices;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.CommandProcessors;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.CommandProcessors
{
    /// <summary>
    /// Command processor to stop telemetry data
    /// </summary>
    public class StopCommandProcessor : CommandProcessor 
    {
        private const string STOP_TELEMETRY = "StopTelemetry";

        public StopCommandProcessor(EngineDevice device)
            : base(device)
        {

        }

        public async override Task<CommandProcessingResult> HandleCommandAsync(DeserializableCommand deserializableCommand)
        {
            if (deserializableCommand.CommandName == STOP_TELEMETRY)
            {
                var command = deserializableCommand.Command;

                try
                {
                    var device = Device as EngineDevice;
                    device.StopTelemetryData();
                    return CommandProcessingResult.Success;
                }
                catch (Exception)
                {
                    return CommandProcessingResult.RetryLater;
                }
            }
            else if (NextCommandProcessor != null)
            {
                return await NextCommandProcessor.HandleCommandAsync(deserializableCommand);
            }

            return CommandProcessingResult.CannotComplete;
        }
    }
}
