using Microsoft.AspNetCore.Mvc;

namespace Squido.Controllers;

public class ContactMvcController : Controller
{
    private readonly ILogger<ContactMvcController> _logger;

    public ContactMvcController(ILogger<ContactMvcController> logger)
    {
        _logger = logger;
    }

    public IActionResult Contact()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        ViewBag.IsLoggedIn = !string.IsNullOrEmpty(token);
        return View();
    }
    
}