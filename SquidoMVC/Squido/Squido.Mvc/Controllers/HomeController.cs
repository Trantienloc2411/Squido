using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using HomeViewModal = SharedViewModal.ViewModels.HomeViewModal;

namespace Squido.Controllers;

public class HomeController(IHttpClientFactory httpClientFactory) : Controller
{
    public List<BookViewModel>? BooksList { get; set; }
    public List<CategoryViewModel>? CategoriesList { get; set; }

    public async Task<ViewResult> Index()
    {
        try
        {
            var client = httpClientFactory.CreateClient("Squido");
            var responseBook = await client.GetAsync("/api/Book");
            var responseCategory = await client.GetAsync("/api/Category");

            if (responseCategory.IsSuccessStatusCode && responseBook.IsSuccessStatusCode)
            {
                var contentBook = await responseBook.Content.ReadAsStringAsync();
                var contentCategory = await responseCategory.Content.ReadAsStringAsync();
                BooksList = JsonSerializer.Deserialize<List<BookViewModel>>(contentBook,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                CategoriesList = JsonSerializer.Deserialize<List<CategoryViewModel>>(contentCategory,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }


            var viewModal = new HomeViewModal
            {
                Categories = CategoriesList,
                Books = BooksList
            };

            return View(viewModal);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string? Keyword { get; set; }

    public async Task<ViewResult> Search(string? keyword, int currentPage = 1)
    {
        try
        {
            var client = httpClientFactory.CreateClient("Squido");

            var responseBook = await client.GetAsync($"/api/Book/?keyword={keyword}&page={currentPage}&pageSize=9");
            var responseCategory = await client.GetAsync("/api/Category");

            var pagingationData = new PaginationViewModel<BookViewModel>();

            if (responseBook.IsSuccessStatusCode)
            {
                var contentPaging = await responseBook.Content.ReadAsStringAsync();
                pagingationData = JsonSerializer.Deserialize<PaginationViewModel<BookViewModel>>(contentPaging,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (responseCategory.IsSuccessStatusCode)
            {
                var contentCategory = await responseCategory.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(contentCategory,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                pagingationData.Categories = categories;
            }

            ViewBag.Keyword = keyword;
            return View("Store", pagingationData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }

    public async Task<IActionResult> Store(int currentPage = 1)
    {
        try
        {
            var client = httpClientFactory.CreateClient("Squido");

            var responseBook = await client.GetAsync($"/api/Book/?page={currentPage}&pageSize=9");
            var responseCategory = await client.GetAsync("/api/Category");

            var pagingationData = new PaginationViewModel<BookViewModel>();

            if (responseBook.IsSuccessStatusCode)
            {
                var contentPaging = await responseBook.Content.ReadAsStringAsync();

                pagingationData = JsonSerializer.Deserialize<PaginationViewModel<BookViewModel>>(contentPaging,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (responseCategory.IsSuccessStatusCode)
            {
                var contentCategory = await responseCategory.Content.ReadAsStringAsync();

                var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(contentCategory,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assign categories to the pagination object
                pagingationData.Categories = categories;
            }

            return View(pagingationData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> Filter(List<int> categories, int currentPage = 1)
    {
        try
        {
            // Convert list to comma-separated string
            string categoryIds = categories != null && categories.Any()
                ? string.Join(",", categories)
                : "";
            
            var client = httpClientFactory.CreateClient("Squido");
            var response = await client.GetAsync($"/api/Book/Filter/{categoryIds}?page={currentPage}&pageSize=9");
            var responseCategory = await client.GetAsync("api/Category");

            var book = new PaginationViewModel<BookViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                book = JsonSerializer.Deserialize<PaginationViewModel<BookViewModel>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (responseCategory.IsSuccessStatusCode)
            {
                var content = await responseCategory.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<List<CategoryViewModel>>
                    (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                book.Categories = result;
            }
            ViewBag.Categories = categoryIds;
            return View("Store", book);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}