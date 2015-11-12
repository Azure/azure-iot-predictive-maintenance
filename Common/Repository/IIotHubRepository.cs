using System.Threading.Tasks;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Repository
{
    /// <summary>
    /// Interface to expose methods that can be called against the underlying identity repository
    /// </summary>
    public interface IIotHubRepository
    {
        Task<Azure.Devices.Device> GetIotHubDeviceAsync(string deviceId);
        Task<dynamic> AddDeviceAsync(dynamic device, SecurityKeys securityKeys);
        Task<bool> TryAddDeviceAsync(Azure.Devices.Device oldIotHubDevice);
        Task RemoveDeviceAsync(string deviceId);
        Task<bool> TryRemoveDeviceAsync(string deviceId);
        Task UpdateDeviceEnabledStatusAsync(string deviceId, bool isEnabled);
		Task SendCommand(string deviceId, dynamic command);
        Task<SecurityKeys> GetDeviceKeysAsync(string id);
    }
}
