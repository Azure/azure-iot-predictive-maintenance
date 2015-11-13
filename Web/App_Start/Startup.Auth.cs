// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------


namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web
{
    using System.Diagnostics;
    using System.IdentityModel.Tokens;
    using System.Web.Http;
    using global::Owin;
    using Owin.Security;
    using Owin.Security.ActiveDirectory;
    using Owin.Security.Cookies;
    using Owin.Security.WsFederation;

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            string federationMetadataAddress = CloudConfigurationManager.GetSetting("FederationMetadataAddress");
            string federationRealm = CloudConfigurationManager.GetSetting("FederationRealm");
            string aadTenant = CloudConfigurationManager.GetSetting("AADTenant");
            string aadAudience = CloudConfigurationManager.GetSetting("AADAudience");

            if (string.IsNullOrWhiteSpace(federationMetadataAddress) || string.IsNullOrWhiteSpace(federationRealm))
            {
                Trace.TraceWarning("Unable to load federation values from web.config or other configuration source. Authentication disabled.");
                return;
            }
            if (string.IsNullOrWhiteSpace(aadTenant) || string.IsNullOrWhiteSpace(aadAudience))
            {
                Trace.TraceWarning("Unable to load AAD values from web.config or other configuration source. Authentication disabled.");
                return;
            }

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            // Primary authentication method for web site to Azure AD via the WsFederation below
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions { MetadataAddress = federationMetadataAddress, Wtrealm = federationRealm });

            // Fallback authentication method to allow "Authorization: Bearer <token>" in the header for WebAPI calls
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                    {
                        Tenant = aadTenant,
                        TokenValidationParameters = new TokenValidationParameters { ValidAudience = aadAudience, RoleClaimType = "roles" },
                    });

            // Require authorization for all controllers
            Startup.HttpConfiguration.Filters.Add(new AuthorizeAttribute());
        }
    }
}