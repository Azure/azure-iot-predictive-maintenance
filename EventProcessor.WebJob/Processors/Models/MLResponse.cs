namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models
{
    using System.Collections.Generic;

    public class MLResponse
    {
        public class Data
        {
            public class Value
            {
                public string[] ColumnNames;
                public string[] ColumnTypes;
                public string[,] Values;
            }

            public string Type;

            public Value value;
        }

        public Dictionary<string, Data> Results;
        public int ContainerAllocationDurationMs;
        public int ExecutionDurationMs;
        public bool IsWarmContainer;
    }
}