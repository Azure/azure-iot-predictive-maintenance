namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Configurations;
    using RClient;
    using RClient.Models;
    using Newtonsoft.Json;

    public class MRSServiceInvoker : IAnalyticsServiceInvoker
    {
        private readonly IConfigurationProvider _configurationProvider;
        private PredictiveMaintenance pmRClient;

        private DateTime refreshTime = DateTime.MinValue;
        private readonly object tokenSyncRoot = new object();

        public MRSServiceInvoker(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            var baseUri = new Uri(_configurationProvider.GetConfigurationSettingValue("MRSApiUrl"));
            pmRClient = new PredictiveMaintenance(baseUri);
        }

        public async Task<string> GetRULAsync(string deviceId, string cycle, string sensor9, string sensor11, string sensor14, string sensor15)
        {
            RefreshAccessTokenIfNeeded();
            try
            {
                var result = await pmRClient.PredictiveAsync(new InputParameters
                {
                    Cycle = int.Parse(cycle),
                    S9 = double.Parse(sensor9),
                    S11 = double.Parse(sensor11),
                    S14 = double.Parse(sensor14),
                    S15 = double.Parse(sensor15)
                });
                var rs = JsonConvert.DeserializeObject<Answer>(result.OutputParameters.Answer.ToString());
                return rs.Score[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception($"The MRS requet failed: {e.ToString()}");
            }
        }

        private void RefreshAccessTokenIfNeeded()
        {
            if (DateTime.Now - refreshTime > TimeSpan.FromMinutes(40))
            {
                lock (tokenSyncRoot)
                {
                    if (DateTime.Now - refreshTime > TimeSpan.FromMinutes(40))
                    {
                        var password = _configurationProvider.GetConfigurationSettingValue("MRSPassword");
                        var token = pmRClient.Login(new LoginRequest { Username = "admin", Password = password });
                        pmRClient.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                    }
                }
            }
        }

        public class Answer
        {
            public List<double> Score { get; set; }
        }
    }
}
