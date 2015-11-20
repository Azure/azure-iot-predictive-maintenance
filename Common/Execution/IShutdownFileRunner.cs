using System;
using System.Threading;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Execution
{
    public interface IShutdownFileRunner
    {
        void Run(Action start, CancellationTokenSource cancellationTokenSource);
    }
}
