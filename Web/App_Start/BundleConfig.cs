// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/vendor")
                .Include("~/scripts/vendor/jquery-{version}.js")
                .Include("~/scripts/vendor/knockout.js")
                .Include("~/scripts/vendor/powerbi-visuals.min.js")
                .Include("~/scripts/vendor/moment-with-locales.js"));
        }
    }
}