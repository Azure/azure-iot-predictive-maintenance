namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Web;
    using System.Web.Mvc;
    using Helpers;
    using Security;
    using ViewModels;

    [Authorize]
    public sealed class HomeController : Controller
    {
        public ActionResult Index()
        {
            var currentCulture = CultureHelper.GetCurrentCulture();
            var languages = new Collection<LanguageModel>();

            foreach (var culture in CultureHelper.GetImplementedCultures())
            {
                languages.Add(new LanguageModel { Name = culture.NativeName, CultureName = culture.Name });
            }

            var model = new IndexModel
            {
                Username = PrincipalHelper.GetUsername(),
                AvailableLanguages = languages,
                CurrentLanguageNameIso = currentCulture.TwoLetterISOLanguageName,
                CurrentLanguageName = currentCulture.NativeName,
                CurrentLanguageTextDirection = currentCulture.TextInfo.IsRightToLeft ? "rtl" : "ltr"
            };

            return View(model);
        }

        [HttpGet]
        [Route("culture/{cultureName}")]
        public ActionResult SetCulture(string cultureName)
        {
            // Save culture in a cookie
            HttpCookie cookie = this.Request.Cookies[Constants.CultureCookieName];

            if (cookie != null)
            {
                cookie.Value = cultureName; // update cookie value
            }
            else
            {
                cookie = new HttpCookie(Constants.CultureCookieName);
                cookie.Value = cultureName;
                cookie.Expires = DateTime.Now.AddYears(1);
            }

            Response.Cookies.Add(cookie);

            return RedirectToAction("Index");
        }
    }
}