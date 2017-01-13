namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    using System;
    using System.Threading.Tasks;
    using Models;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;
    using Common.Configurations;

    public class AMLServiceInvoker : IAnalyticsServiceInvoker
    {
        readonly IConfigurationProvider _configurationProvider;

        const string ML_ENDPOINT = "/execute?api-version=2.0&details=true";
        readonly string[] ML_REQUEST_COLUMNS = { "id", "cycle", "s9", "s11", "s14", "s15" };
        const int RUL_COLUMN = 2;

        public AMLServiceInvoker(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        public async Task<string> GetRULAsync(string deviceId, string cycle, string sensor9, string sensor11, string sensor14, string sensor15)
        {
            // The experiment theoretically supports multiple inputs at once,
            // even though we only get one value at a time, so the request
            // requires an array of inputs
            MLRequest mlRequest = new MLRequest(ML_REQUEST_COLUMNS, new string[,]
            {
                {
                    // The id is required to be numeric, so we hash the actual device id
                    deviceId,
                    // The remaining entries are string representations of the numeric values
                    cycle,
                    sensor9,
                    sensor11,
                    sensor14,
                    sensor15
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
                return result.Results["data"].value.Values[0, RUL_COLUMN];
            }
            else
            {
                throw new Exception(string.Format("The AML request failed with status code: {0}", response.StatusCode));
            }
        }
    }
}
