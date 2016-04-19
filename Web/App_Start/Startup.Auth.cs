namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using Common.Configurations;
    using global::Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using System.Configuration;
    using System.Globalization;
    using Microsoft.Owin.Security.OpenIdConnect;
    using System.Threading.Tasks;

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app, IConfigurationProvider configProvider)
        {
            string aadClientId = configProvider.GetConfigurationSettingValue("ida.AADClientId");
            string aadInstance = configProvider.GetConfigurationSettingValue("ida.AADInstance");
            string aadTenant = configProvider.GetConfigurationSettingValue("ida.AADTenant");
            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, aadTenant);

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = aadClientId,
                Authority = authority,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;

                        context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                        context.HandleResponse();
                        context.Response.Redirect(context.ProtocolMessage.RedirectUri);

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
