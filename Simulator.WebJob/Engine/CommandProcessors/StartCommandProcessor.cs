namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.Engine.CommandProcessors
{
    using System;
    using System.Threading.Tasks;
    using Devices;
    using SimulatorCore.CommandProcessors;
    using SimulatorCore.Transport;

    /// <summary>
    /// Command processor to start telemetry data
    /// </summary>
    public class StartCommandProcessor : CommandProcessor
    {
        const string START_TELEMETRY = "StartTelemetry";

        public StartCommandProcessor(EngineDevice device)
            : base(device)
        {
        }

        public override async Task<CommandProcessingResult> HandleCommandAsync(DeserializableCommand deserializableCommand)
        {
            if (deserializableCommand.CommandName == START_TELEMETRY)
            {
                var command = deserializableCommand.Command;

                try
                {
                    var device = Device as EngineDevice;
                    device.StartTelemetryData();
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