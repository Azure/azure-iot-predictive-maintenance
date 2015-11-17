// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Security
{
    using System.Security.Claims;
    using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;

    public static class PrincipalHelper
    {
        public static string GetUsername()
        {
            var emailClaim = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);

            return emailClaim.Value;
        }
    }
}