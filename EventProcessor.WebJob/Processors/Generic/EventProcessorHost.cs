using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Generic
{
    public class EventProcessorHost<TEventProcessorFactory> : IEventProcessorHost, IDisposable where TEventProcessorFactory : class, IEventProcessorFactory
    {
        private readonly object[] _arguments;
        private readonly string _eventHubName;
        private readonly string _eventHubConnectionString;
        private readonly string _storageConnectionString;

        private EventProcessorHost _eventProcessorHost = null;
        private TEventProcessorFactory _factory;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _running = false;
        private bool _disposed = false;

        public EventProcessorHost(string eventHubName, string eventHubConnectionString, string storageConnectionString, params object[] arguments)
        {
            _arguments = arguments;
            _eventHubName = eventHubName;
            _eventHubConnectionString = eventHubConnectionString;
            _storageConnectionString = storageConnectionString;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            this.Start(this._cancellationTokenSource.Token);
        }

        public void Start(CancellationToken cancellationToken)
        {
            _running = true;
            Task.Run(() => this.StartProcessor(cancellationToken), cancellationToken);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            TimeSpan sleepInterval = TimeSpan.FromSeconds(1);
            while (_running)
            {
                if (timeout < sleepInterval)
                {
                    break;
                }
                Thread.Sleep(sleepInterval);
            }
        }

        public async Task StartProcessor(CancellationToken token)
        {
            try
            {
                // Initialize
                _eventProcessorHost = new EventProcessorHost(
                    Environment.MachineName,
                    _eventHubName.ToLowerInvariant(),
                    EventHubConsumerGroup.DefaultGroupName,
                    _eventHubConnectionString,
                    _storageConnectionString);

                _factory = Activator.CreateInstance(typeof(TEventProcessorFactory), _arguments) as TEventProcessorFactory;

                Trace.TraceInformation("{0}: Registering host...", this.GetType().Name);

                EventProcessorOptions options = new EventProcessorOptions();
                options.ExceptionReceived += OptionsOnExceptionReceived;
                await _eventProcessorHost.RegisterEventProcessorFactoryAsync(_factory);

                // processing loop
                while (!token.IsCancellationRequested)
                {
                    Trace.TraceInformation("{0}: Processing...", this.GetType().Name);
                    await Task.Delay(TimeSpan.FromMinutes(5), token);
                }

                // cleanup
                await _eventProcessorHost.UnregisterEventProcessorAsync();
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Error in {0}.StartProcessor, Exception: {1}", this.GetType().Name, e.Message);
            }
            _running = false;
        }

        private void OptionsOnExceptionReceived(object sender, ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Trace.TraceError("Received exception, action: {0}, message: {1}", exceptionReceivedEventArgs.Action, exceptionReceivedEventArgs.Exception.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                }
            }

            _disposed = true;
        }

        ~EventProcessorHost()
        {
            Dispose(false);
        }
    }
}
