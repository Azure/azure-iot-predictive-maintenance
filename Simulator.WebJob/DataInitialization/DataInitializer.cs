namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.DataInitialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Common.Configurations;
    using Common.DeviceSchema;
    using Common.Factory;
    using Common.Models;
    using Common.Repository;

    public class DataInitializer : IDataInitializer
    {
        readonly IIotHubRepository _iotHubRepository;
        readonly IVirtualDeviceStorage _virtualDeviceStorage;
        readonly IConfigurationProvider _configProvider;
        readonly ISecurityKeyGenerator _securityKeyGenerator;

        public DataInitializer(
            IIotHubRepository iotHubRepository,
            IVirtualDeviceStorage virtualDeviceStorage,
            ISecurityKeyGenerator securityKeyGenerator,
            IConfigurationProvider configProvider)
        {
            _iotHubRepository = iotHubRepository;
            _virtualDeviceStorage = virtualDeviceStorage;
            _securityKeyGenerator = securityKeyGenerator;
            _configProvider = configProvider;
        }

        public void CreateInitialDataIfNeeded()
        {
            try
            {
                bool initializationNeeded = false;

                // only create default data if the action mappings are missing

                // check if any devices are there
                Task<bool>.Run(async () => initializationNeeded = (await _virtualDeviceStorage.GetDeviceListAsync()).Count == 0).Wait();

                if (!initializationNeeded)
                {
                    Trace.TraceInformation("No initial data needed.");
                    return;
                }

                Trace.TraceInformation("Beginning initial data creation...");

                List<string> bootstrappedDevices = null;

                // create default devices
                Task.Run(async () => bootstrappedDevices = await BootstrapDefaultDevices()).Wait();

                Trace.TraceInformation("Initial data creation completed.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to create initial default data: {0}", ex.ToString());
            }
        }

        async Task<List<string>> BootstrapDefaultDevices()
        {
            List<string> sampleIds = SampleDeviceFactory.GetDefaultDeviceNames();
            foreach (string id in sampleIds)
            {
                dynamic device = DeviceSchemaHelper.BuildDeviceStructure(id, true);
                SecurityKeys securityKeys = _securityKeyGenerator.CreateRandomKeys();
                try
                {
                    await _iotHubRepository.AddDeviceAsync(device, securityKeys);
                    await _virtualDeviceStorage.AddOrUpdateDeviceAsync(new InitialDeviceConfig
                    {
                        DeviceId = DeviceSchemaHelper.GetDeviceID(device),
                        HostName = _configProvider.GetConfigurationSettingValue("iotHub.HostName"),
                        Key = securityKeys.PrimaryKey
                    });
                }
                catch (Exception ex)
                {
                    //if we fail adding to table storage for the device simulator just continue
                    Trace.TraceError("Failed to add simulated device : {0}", ex.Message);
                }
            }
            return sampleIds;
        }
    }
}