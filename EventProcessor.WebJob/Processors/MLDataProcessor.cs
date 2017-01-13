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
    using MachineLearning;
    using System.Diagnostics;

    public class MLDataProcessor : EventProcessor
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly IAnalyticsServiceInvoker _mlServiceInvoker;

        public MLDataProcessor(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            var analyticsType = (AnalyticsTypes)Enum.Parse(typeof(AnalyticsTypes), _configurationProvider.GetConfigurationSettingValue("AnalyticsType"));
            _mlServiceInvoker = AnalyticsServiceInvokerFactory.CreateMLServerInvoker(analyticsType, configurationProvider);
            Trace.TraceInformation($"Created ML service invoker of type: {_mlServiceInvoker.GetType().Name}");
        }

        public override async Task ProcessItem(dynamic eventData)
        {
            // Ensure this is a correctly-formatted event for ML; ignore it otherwise
            if (eventData == null || eventData.deviceid == null || eventData.cycle == null ||
                eventData.sensor9 == null || eventData.sensor11 == null || eventData.sensor14 == null || eventData.sensor15 == null)
            {
                return;
            }

            string result = await _mlServiceInvoker.GetRULAsync(
                // The id is required to be numeric, so we hash the actual device id
                eventData.deviceid.ToString().GetHashCode().ToString(),
                // The remaining entries are string representations of the numeric values
                eventData.cycle.ToString(),
                eventData.sensor9.ToString(),
                eventData.sensor11.ToString(),
                eventData.sensor14.ToString(),
                eventData.sensor15.ToString()
            );

            Trace.TraceInformation($"RUL Result: {result}");

            RulTableEntity entry = new RulTableEntity
            {
                PartitionKey = eventData.deviceid.ToString(),
                RowKey = eventData.cycle.ToString(),
                // Extract the single relevant RUL value from the JSON output
                Rul = result,
                // Since the simulator might replay data, ensure we can overwrite table values
                ETag = "*"
            };

            // We don't need a data model to represent the result of this operation,
            // so we use a stub table/model convertor
            await AzureTableStorageHelper.DoTableInsertOrReplaceAsync<object, RulTableEntity>(entry, (RulTableEntity e) => null,
                _configurationProvider.GetConfigurationSettingValue("eventHub.StorageConnectionString"),
                _configurationProvider.GetConfigurationSettingValue("MLResultTableName"));
        }
    }
}