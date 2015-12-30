namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Execution
{
    using System;
    using System.Threading;

    public interface IShutdownFileWatcher
    {
        void Run(Action start, CancellationTokenSource cancellationTokenSource);
    }
}