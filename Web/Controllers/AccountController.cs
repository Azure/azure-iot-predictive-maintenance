// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Controller for handling account related actions.
    /// </summary>
    public sealed class AccountController : Controller
    {
        /// <summary>
        /// Signs the user out.
        /// </summary>
        public ActionResult SignOut()
        {
            this.HttpContext.GetOwinContext().Authentication.SignOut();

            return this.RedirectToAction("Index", "Device");
        }
    }
}