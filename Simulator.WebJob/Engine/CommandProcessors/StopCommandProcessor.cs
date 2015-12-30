namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.CommandProcessors
{
    using System;
    using System.Threading.Tasks;
    using Devices;
    using SimulatorCore.CommandProcessors;
    using SimulatorCore.Transport;

    /// <summary>
    /// Command processor to stop telemetry data
    /// </summary>
    public class StopCommandProcessor : CommandProcessor
    {
        const string STOP_TELEMETRY = "StopTelemetry";

        public StopCommandProcessor(EngineDevice device)
            : base(device)
        {
        }

        public override async Task<CommandProcessingResult> HandleCommandAsync(DeserializableCommand deserializableCommand)
        {
            if (deserializableCommand.CommandName == STOP_TELEMETRY)
            {
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