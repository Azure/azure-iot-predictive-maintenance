namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Telemetry
    {
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

        [DataMember(Name = "recordId")]
        public string RecordId { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; }

        [DataMember(Name = "sensor1")]
        public double Sensor1 { get; set; }

        [DataMember(Name = "sensor2")]
        public double Sensor2 { get; set; }

        [DataMember(Name = "sensor3")]
        public double Sensor3 { get; set; }

        [DataMember(Name = "sensor4")]
        public double Sensor4 { get; set; }
    }
}