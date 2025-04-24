using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Squido.Controllers
{
    public class CheckoutMvcController : BaseController
    {
        public IActionResult Index()
        {
            if(!ViewBag.IsLoggedIn) {
                TempData["ErrorLogin"] = "For best experience please login first";
                return RedirectToAction("Index", "AuthMvc");
            }
            return View("Views/Checkout/Index.cshtml");
        }
    }
}
