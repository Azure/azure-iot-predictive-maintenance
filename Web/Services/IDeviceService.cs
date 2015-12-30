namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System.Collections.Generic;

    public interface IDeviceService
    {
        IEnumerable<string> GetDeviceIds();
    }
}