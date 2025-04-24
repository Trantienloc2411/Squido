using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Squido.Controllers
{

    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(token);
            base.OnActionExecuting(context);
        }

        protected IActionResult RedirectToLoginIfNotLoggedIn()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorLogin"] = "Please login first to continue.";
                return RedirectToAction("Index", "AuthMvc");
            }

            return null;
        }

    }
}
