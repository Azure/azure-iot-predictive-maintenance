// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public sealed class HomeController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}