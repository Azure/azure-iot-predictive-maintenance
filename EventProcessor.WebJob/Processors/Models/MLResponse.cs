using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models
{
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

            public string type;
            public Value value;
        }

        public Dictionary<string, Data> Results;
        public int ContainerAllocationDurationMs;
        public int ExecutionDurationMs;
        public bool IsWarmContainer;
    }
}
