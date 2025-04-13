using Microsoft.AspNetCore.Mvc;
using Squido.Services.Interfaces;

namespace Squido.ViewComponents;

public class CategoryViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;
    public CategoryViewComponent(ICategoryService categoryService) => _categoryService = categoryService;
    public IViewComponentResult Invoke()
    { 
        var categories = _categoryService.GetCategories().ToList();
        return View(categories);
    }
}