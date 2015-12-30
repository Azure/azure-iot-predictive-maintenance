namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Security
{
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;

    public static class PrincipalHelper
    {
        public static string GetUsername()
        {
            var principal = HttpContext.Current.User;

            // for some account types, this is the email
            if (principal.Identity.Name != null)
            {
                return principal.Identity.Name;
            }

            // if that didn't work, try to cast into a ClaimsPrincipal
            var claimsPrincipal = principal as ClaimsPrincipal;

            if (claimsPrincipal == null || claimsPrincipal.Claims == null)
            {
                // no email available
                return "";
            }

            // try to fish out the email claim
            var emailAddressClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailAddressClaim == null)
            {
                return "";
            }

            return emailAddressClaim.Value;
        }
    }
}