// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Async;
    using Newtonsoft.Json;

    public abstract class ApiController : AsyncController
    {
        protected ApiController()
        {
            this.ActionInvoker = new WebApiActionInvoker();
        }
    }

    public sealed class WebApiActionInvoker : AsyncControllerActionInvoker
    {
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, Object actionReturnValue)
        {
            if (actionReturnValue == null)
            {
                return new EmptyResult();
            }

            var actionResult = actionReturnValue as ActionResult;

            if (actionResult == null)
            {
                var contentResult = new ContentResult();
                contentResult.Content = JsonConvert.SerializeObject(actionReturnValue);
                contentResult.ContentType = "application/json";

                return contentResult;
            }

            return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }
    }
}