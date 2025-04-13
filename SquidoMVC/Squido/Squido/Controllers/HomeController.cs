using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Squido.Models;
using Squido.Models.ViewModals;
using Squido.Services.Interfaces;
using Squido.Services.Services;

namespace Squido.Controllers;

public class HomeController : Controller
{
    

    private readonly ICategoryService _categoryService;
    private readonly IBookService _bookService;

    public HomeController(ICategoryService categoryService, IBookService bookService)
    {
        _categoryService = categoryService;
        _bookService = bookService;
    }

    public async Task<ViewResult> Index()
    {
        var categories = _categoryService.GetCategories().ToList();
        var books = await _bookService.GetBooks();

        var viewModal = new HomeViewModal
        {
            Categories = categories,
            Books = books
        };
        
        return View(viewModal);
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }

    public IActionResult Store()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}