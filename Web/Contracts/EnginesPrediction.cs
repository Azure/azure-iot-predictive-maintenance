namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Contracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class EnginesPrediction
    {
        [DataMember(Name = "engine1prediction")]
        public IEnumerable<Prediction> Engine1Prediction { get; set; }

        [DataMember(Name = "engine2prediction")]
        public IEnumerable<Prediction> Engine2Prediction { get; set; }
    }
}