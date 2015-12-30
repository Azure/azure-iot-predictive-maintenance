namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Bundling
{
    using System.IO;
    using System.Web.Optimization;
    using dotless.Core;

    public sealed class LessTransform : IBundleTransform
    {
        readonly string _path;

        public LessTransform(string path)
        {
            this._path = path;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            var oldPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(this._path);

            response.Content = Less.Parse(response.Content);
            Directory.SetCurrentDirectory(oldPath);
            response.ContentType = "text/css";
        }
    }
}