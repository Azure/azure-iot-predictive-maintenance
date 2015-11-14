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
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/scripts/vendor/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout")
                .Include("~/scripts/vendor/knockout.js"));

            bundles.Add(new ScriptBundle("~/bundles/powerbi")
                .Include("~/scripts/vendor/powerbi-visuals.min.js"));
        }
    }
}