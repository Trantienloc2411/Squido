using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var cat = await categoryService.GetCategoryById(id);
            if (cat.IsSuccess) return Ok(cat.Data);
            return BadRequest(cat.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
    {
        try
        {
            var result = await categoryService.CreateCategory(model);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CreateCategoryModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await categoryService.UpdateCategory(id, model);
            if (result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var result = await categoryService.DeleteCategory(id);
            if(result.IsSuccess) return Ok(result.Data);
            return BadRequest(result.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

}