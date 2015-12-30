namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Factory
{
    using System.Collections.Generic;
    using DeviceSchema;

    public static class SampleDeviceFactory
    {
        public const string OBJECT_TYPE_DEVICE_INFO = "DeviceInfo";

        public const string VERSION_1_0 = "1.0";

        const bool IS_SIMULATED_DEVICE = true;

        static readonly List<string> DefaultDeviceNames = new List<string>
        {
            "N1172FJ-1",
            "N1172FJ-2",
            "N2172FJ-1",
            "N2172FJ-2"
        };

        public static dynamic GetSampleSimulatedDevice(string deviceId, string key)
        {
            dynamic device = DeviceSchemaHelper.BuildDeviceStructure(deviceId, true);

            AssignDeviceProperties(deviceId, device);
            device.ObjectType = OBJECT_TYPE_DEVICE_INFO;
            device.Version = VERSION_1_0;
            device.IsSimulatedDevice = IS_SIMULATED_DEVICE;

            AssignCommands(device);

            return device;
        }

        static void AssignDeviceProperties(string deviceId, dynamic device)
        {
            dynamic deviceProperties = DeviceSchemaHelper.GetDeviceProperties(device);
            deviceProperties.HubEnabledState = true;
            deviceProperties.Manufacturer = "Fabrikam";
            deviceProperties.DeviceName = string.Join(" Engine #", deviceId.Split('-'));
            deviceProperties.ModelNumber = "FB-27b";
            deviceProperties.SerialNumber = "FB" + GetIntBasedOnString(deviceId + "SerialNumber", 10000);
        }

        static int GetIntBasedOnString(string input, int maxValueExclusive)
        {
            int hash = input.GetHashCode();

            //Keep the result positive
            if (hash < 0)
            {
                hash = -hash;
            }

            return hash % maxValueExclusive;
        }

        static void AssignCommands(dynamic device)
        {
            dynamic command = CommandSchemaHelper.CreateNewCommand("PingDevice");
            CommandSchemaHelper.AddCommandToDevice(device, command);

            command = CommandSchemaHelper.CreateNewCommand("StartTelemetry");
            CommandSchemaHelper.AddCommandToDevice(device, command);

            command = CommandSchemaHelper.CreateNewCommand("StopTelemetry");
            CommandSchemaHelper.AddCommandToDevice(device, command);
        }

        public static List<string> GetDefaultDeviceNames()
        {
            return DefaultDeviceNames;
        }
    }
}