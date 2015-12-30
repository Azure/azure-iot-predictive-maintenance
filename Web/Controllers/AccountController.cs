namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Web;
    using System.Web.Mvc;

    public sealed class AccountController : Controller
    {
        /// <summary>
        /// Signs the user out.
        /// </summary>
        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            return RedirectToAction("Index", "Device");
        }
    }
}