using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    

    // GET
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await categoryService.GetCategories());
    }
}