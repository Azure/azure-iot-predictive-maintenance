// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Device
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }

        [DataMember(Name = "modelNumber")]
        public string ModelNumber { get; set; }

        [DataMember(Name = "serialNumber")]
        public string SerialNumber { get; set; }

        [DataMember(Name = "firmware")]
        public string Firmware { get; set; }

        [DataMember(Name = "platform")]
        public string Platform { get; set; }

        [DataMember(Name = "processor")]
        public string Processor { get; set; }

        [DataMember(Name = "memory")]
        public string Memory { get; set; }

        [DataMember(Name = "updatedTime")]
        public string UpdatedTime { get; set; }

        [DataMember(Name = "createdTime")]
        public string CreatedTime { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "hubEnabledState")]
        public bool HubEnabledState { get; set; }

        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }
    }
}