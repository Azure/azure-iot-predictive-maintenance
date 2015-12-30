namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Client;
    using Logging;
    using Serialization;

    public class EmptyTransport : ITransport
    {
        readonly ILogger _logger;
        readonly ISerialize _serializer;

        public EmptyTransport(ILogger logger, ISerialize serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        public void Open()
        {
        }

        public Task CloseAsync()
        {
            return Task.Run(() => { });
        }

        public async Task SendEventAsync(dynamic eventData)
        {
            var eventId = Guid.NewGuid();
            await SendEventAsync(eventId, eventData);
        }

        public async Task SendEventAsync(Guid eventId, dynamic eventData)
        {
            _logger.LogInfo("SendEventAsync called:");
            _logger.LogInfo("SendEventAsync: EventId: " + eventId);
            _logger.LogInfo("SendEventAsync: message: " + eventData.ToString());

            await Task.Run(() => { });
        }

        public async Task SendEventBatchAsync(IEnumerable<Message> messages)
        {
            _logger.LogInfo("SendEventBatchAsync called");

            await Task.Run(() => { });
        }

        public async Task<DeserializableCommand> ReceiveAsync()
        {
            _logger.LogInfo("ReceiveAsync: waiting...");
            return await Task.Run(() => new DeserializableCommand(new Message(), _serializer));
        }

        public async Task SignalAbandonedCommand(DeserializableCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            await Task.Run(() => { });
        }

        public async Task SignalCompletedCommand(DeserializableCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            await Task.Run(() => { });
        }

        public async Task SignalRejectedCommand(DeserializableCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            await Task.Run(() => { });
        }
    }
}