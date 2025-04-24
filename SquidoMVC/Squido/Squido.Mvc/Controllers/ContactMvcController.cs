using Microsoft.AspNetCore.Mvc;

namespace Squido.Controllers;

public class ContactMvcController : BaseController
{
    private readonly ILogger<ContactMvcController> _logger;

    public ContactMvcController(ILogger<ContactMvcController> logger)
    {
        _logger = logger;
    }

    public IActionResult Contact()
    {
        return View();
    }
    
}