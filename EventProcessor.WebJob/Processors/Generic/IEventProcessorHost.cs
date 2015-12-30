namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Generic
{
    using System.Threading;

    public interface IEventProcessorHost
    {
        void Start();

        void Start(CancellationToken token);

        void Stop();
    }
}