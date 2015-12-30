namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System.Collections.Generic;

    public sealed class DeviceService : IDeviceService
    {
        public IEnumerable<string> GetDeviceIds()
        {
            return new[] { "N2172FJ-1", "N2172FJ-2" };
        }
    }
}