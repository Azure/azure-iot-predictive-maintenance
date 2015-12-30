namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Owin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    public sealed class EnforceHttpsMiddleware : OwinMiddleware
    {
        public EnforceHttpsMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            if (!context.Request.IsSecure && context.Request.Uri.Host != "127.0.0.1")
            {
                if (!string.Equals(context.Request.Method, "GET", StringComparison.Ordinal))
                {
                    var message = string.Format("Https is required. HostName: {0}. Method: {1}", context.Request.Uri.Host, context.Request.Method);
                    throw new SecurityException(message);
                }

                var uri = context.Request.Uri;

                var uriBuilder = new UriBuilder(uri)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = 443
                };
                context.Response.Redirect(uriBuilder.ToString());
                return Task.FromResult(true);
            }
            context.Response.Headers.Set("Strict-Transport-Security", "max-age=31536000"); //31536000 - approx. one year

            return Next.Invoke(context);
        }
    }
}