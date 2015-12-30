namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Prediction
    {
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; }

        [DataMember(Name = "rul")]
        public int RemainingUsefulLife { get; set; }

        [DataMember(Name = "cycles")]
        public int Cycles { get; set; }
    }
}