using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Helpers;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    public class MLDataProcessor : Generic.EventProcessor
    {
        private const string ML_ENDPOINT = "/execute?api-version=2.0&details=true";

        private const int RUL_COLUMN = 2;

        private readonly IConfigurationProvider _configurationProvider;

        public MLDataProcessor(IConfigurationProvider configurationProvider) : base()
        {
            _configurationProvider = configurationProvider;
        }

        public override async Task ProcessItem(dynamic eventData)
        {
            // Create a generic object to represent the JSON request required by this ML experiment;
            // this converts the JSON input (from ASA) into the required format
            var mlRequest = new {
                Inputs = new {
                    data = new {
                        ColumnNames = new string[] {"id", "cycle", "s9", "s11", "s14", "s15"},
                        // The experiment theoretically supports multiple inputs at once,
                        // even though we only get one value at a time, so this is an array of inputs
                        Values = new string[,] { {
                            // The id is required to be numeric, so we hash the actual device id
                            eventData.deviceid.ToString().GetHashCode().ToString(),
                            // The remaining entries are string representations of the numeric values
                            eventData.cycle.ToString(),
                            eventData.sensor9.ToString(),
                            eventData.sensor11.ToString(),
                            eventData.sensor14.ToString(),
                            eventData.sensor15.ToString()
                        } }
                    }
                },
                GlobalParameters = new {}
            };

            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configurationProvider.GetConfigurationSettingValue("MLApiKey"));
            http.BaseAddress = new Uri(_configurationProvider.GetConfigurationSettingValue("MLApiUrl") + ML_ENDPOINT);

            HttpResponseMessage response = await http.PostAsJsonAsync("", mlRequest);
            if (response.IsSuccessStatusCode)
            {
                dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                RulTableEntity entry = new RulTableEntity()
                {
                    PartitionKey = eventData.deviceid.ToString(),
                    RowKey = eventData.cycle.ToString(),
                    // Extract the single relevant RUL value from the JSON output
                    Rul = result.Results.data.value.Values[0][RUL_COLUMN].ToString()
                };

                // We don't need a data model to represent the result of this operation,
                // so we use a stub table/model convertor
                await AzureTableStorageHelper.DoTableInsertOrReplaceAsync<object, RulTableEntity>(entry, (RulTableEntity e) => { return null; },
                    _configurationProvider.GetConfigurationSettingValue("eventHub.StorageConnectionString"),
                    _configurationProvider.GetConfigurationSettingValue("MLResultTableName"));
            }
            else
            {
                throw new Exception(string.Format("The ML request failed with status code: {0}", response.StatusCode));
            }
        }
    }
}
