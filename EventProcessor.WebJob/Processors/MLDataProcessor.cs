namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Common.Configurations;
    using Common.Helpers;
    using Generic;
    using Models;
    using Newtonsoft.Json;

    public class MLDataProcessor : EventProcessor
    {
        const string ML_ENDPOINT = "/execute?api-version=2.0&details=true";
        readonly string[] ML_REQUEST_COLUMNS = { "id", "cycle", "s9", "s11", "s14", "s15" };
        const int RUL_COLUMN = 2;

        readonly IConfigurationProvider _configurationProvider;

        public MLDataProcessor(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override async Task ProcessItem(dynamic eventData)
        {
            // Ensure this is a correctly-formatted event for ML; ignore it otherwise
            if (eventData == null || eventData.deviceid == null || eventData.cycle == null ||
                eventData.sensor9 == null || eventData.sensor11 == null || eventData.sensor14 == null || eventData.sensor15 == null)
            {
                return;
            }

            // The experiment theoretically supports multiple inputs at once,
            // even though we only get one value at a time, so the request
            // requires an array of inputs
            MLRequest mlRequest = new MLRequest(ML_REQUEST_COLUMNS, new string[,]
            {
                {
                    // The id is required to be numeric, so we hash the actual device id
                    eventData.deviceid.ToString().GetHashCode().ToString(),
                    // The remaining entries are string representations of the numeric values
                    eventData.cycle.ToString(),
                    eventData.sensor9.ToString(),
                    eventData.sensor11.ToString(),
                    eventData.sensor14.ToString(),
                    eventData.sensor15.ToString()
                }
            }
                );

            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configurationProvider.GetConfigurationSettingValue("MLApiKey"));
            http.BaseAddress = new Uri(_configurationProvider.GetConfigurationSettingValue("MLApiUrl") + ML_ENDPOINT);

            HttpResponseMessage response = await http.PostAsJsonAsync("", mlRequest);
            if (response.IsSuccessStatusCode)
            {
                MLResponse result = JsonConvert.DeserializeObject<MLResponse>(await response.Content.ReadAsStringAsync());

                RulTableEntity entry = new RulTableEntity
                {
                    PartitionKey = eventData.deviceid.ToString(),
                    RowKey = eventData.cycle.ToString(),
                    // Extract the single relevant RUL value from the JSON output
                    Rul = result.Results["data"].value.Values[0, RUL_COLUMN],
                    // Since the simulator might replay data, ensure we can overwrite table values
                    ETag = "*"
                };

                // We don't need a data model to represent the result of this operation,
                // so we use a stub table/model convertor
                await AzureTableStorageHelper.DoTableInsertOrReplaceAsync<object, RulTableEntity>(entry, (RulTableEntity e) => null,
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