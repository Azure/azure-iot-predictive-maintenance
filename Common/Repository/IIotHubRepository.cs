namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Repository
{
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// Interface to expose methods that can be called against the underlying identity repository
    /// </summary>
    public interface IIotHubRepository
    {
        Task<Device> GetIotHubDeviceAsync(string deviceId);

        Task<dynamic> AddDeviceAsync(dynamic device, SecurityKeys securityKeys);

        Task<bool> TryAddDeviceAsync(Device oldIotHubDevice);

        Task RemoveDeviceAsync(string deviceId);

        Task<bool> TryRemoveDeviceAsync(string deviceId);

        Task UpdateDeviceEnabledStatusAsync(string deviceId, bool isEnabled);

        Task SendCommand(string deviceId, dynamic command);

        Task<SecurityKeys> GetDeviceKeysAsync(string id);
    }
}