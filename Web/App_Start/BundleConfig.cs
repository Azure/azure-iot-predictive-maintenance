// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Web;
    using System.Web.Optimization;
    using Bundling;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundle/scripts/")
                .Include("~/scripts/vendor/jquery-{version}.js")
                .Include("~/scripts/vendor/knockout.js")
                .Include("~/scripts/vendor/powerbi-visuals.min.js")
                .Include("~/scripts/vendor/moment-with-locales.js")
                .Include("~/scripts/app.js"));

            var lessBundle = new Bundle("~/bundle/styles/")
                .Include("~/Styles/app.less");

            lessBundle.Transforms.Add(new LessTransform(HttpContext.Current.Server.MapPath("~/styles")));
            lessBundle.Transforms.Add(new CssMinify());

            bundles.Add(lessBundle);
        }
    }
}