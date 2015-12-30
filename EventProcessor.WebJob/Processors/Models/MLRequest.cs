namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models
{
    using System.Collections.Generic;

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
            Inputs = new Dictionary<string, Data>
            {
                {
                    "data",
                    new Data
                    {
                        ColumnNames = columns,
                        Values = values
                    }
                }
            };
            GlobalParameters = new Dictionary<string, string>();
        }
    }
}