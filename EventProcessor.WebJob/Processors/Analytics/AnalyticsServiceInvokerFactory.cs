namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors.MachineLearning
{
    using Common.Configurations;
    public static class AnalyticsServiceInvokerFactory
    {
        public static IAnalyticsServiceInvoker CreateMLServerInvoker(AnalyticsTypes AnalyticsType, IConfigurationProvider configProvider)
        {
            switch (AnalyticsType)
            {
                case AnalyticsTypes.AML:
                    return new AMLServiceInvoker(configProvider);
                case AnalyticsTypes.MRS:
                    return new MRSServiceInvoker(configProvider);
                default:
                    return null;
            }
        }
    }
}
