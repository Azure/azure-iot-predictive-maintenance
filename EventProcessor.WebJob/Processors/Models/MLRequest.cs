using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models
{
    public class MLRequest
    {
        public class Data
        {
            public string[] ColumnNames;
            public string[,] Values;
        }

        public Dictionary<string, Data> Inputs;
        public Dictionary<string, string> GlobalParameters;

        public MLRequest(string[] columns, string[,] values)
        {
            Inputs = new Dictionary<string, Data>()
            { { 
                "data", 
                new Data() 
                {
                    ColumnNames = columns,
                    Values = values
                }
            } };
            GlobalParameters = new Dictionary<string, string>();
        }
    }
}
