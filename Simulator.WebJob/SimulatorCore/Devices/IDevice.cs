namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.SimulatorCore.Devices
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Models;
    using Telemetry;

    /// <summary>
    /// Represents a device. Implementors may be written in managed code, or a managed wrapper
    /// around a native (C/C++) core.
    /// </summary>
    public interface IDevice
    {
        string DeviceID { get; set; }

        string HostName { get; set; }

        string PrimaryAuthKey { get; set; }

        dynamic DeviceProperties { get; set; }

        dynamic Commands { get; set; }

        List<ITelemetry> TelemetryEvents { get; }

        bool RepeatEventListForever { get; set; }

        void Init(InitialDeviceConfig config);

        Task SendDeviceInfo();

        dynamic GetDeviceInfo();

        Task StartAsync(CancellationToken token);
    }
}