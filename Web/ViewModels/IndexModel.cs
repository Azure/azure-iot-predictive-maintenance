namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.ViewModels
{
    using System.Collections.Generic;

    public sealed class IndexModel
    {
        public string Username { get; set; }

        public string AppInsightsKey { get; set; }

        public IEnumerable<LanguageModel> AvailableLanguages { get; set; }

        public string CurrentLanguageNameIso { get; set; }

        public string CurrentLanguageName { get; set; }

        public string CurrentLanguageTextDirection { get; set; }
    }
}