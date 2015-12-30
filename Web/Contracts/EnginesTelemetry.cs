namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Contracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class EnginesTelemetry
    {
        [DataMember(Name = "engine1telemetry")]
        public IEnumerable<Telemetry> Engine1Telemetry { get; set; }

        [DataMember(Name = "engine2telemetry")]
        public IEnumerable<Telemetry> Engine2Telemetry { get; set; }
    }
}