// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Contracts
{
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    [DataContract]
    public sealed class EnginesTelemetry
    {
        [DataMember(Name = "engine1telemetry")]
        public IEnumerable<Telemetry> Engine1Telemetry { get; set; }

        [DataMember(Name = "engine2telemetry")]
        public IEnumerable<Telemetry> Engine2Telemetry { get; set; }
    }
}