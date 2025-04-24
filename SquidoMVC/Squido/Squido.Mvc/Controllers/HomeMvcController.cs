using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;

namespace Squido.Controllers;

public class HomeMvcController(IHttpClientFactory httpClientFactory) : BaseController
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


            var viewModal = new HomeViewModel
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
        ViewBag.CurrentAction = "Search";
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
            ViewBag.Categories = new List<int>();
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
            ViewBag.CurrentAction = "Store"; // Add this
            ViewBag.Categories = new List<int>(); // Add this
            ViewBag.Keyword = null; // Add this

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

    public async Task<IActionResult> Filter(List<int> categories, string keyword = null, int currentPage = 1)
    {
        try
        {
            // Set these before any potential exceptions
            ViewBag.CurrentAction = "Filter";
            ViewBag.Categories = categories ?? new List<int>();
            ViewBag.Keyword = keyword;

            string categoryIds = categories != null && categories.Any()
                ? string.Join(",", categories)
                : string.Empty;

            var client = httpClientFactory.CreateClient("Squido");

            // Include keyword in the filter API call if it exists
            var endpoint = $"/api/Book/Filter/{categoryIds}?page={currentPage}&pageSize=9";
            if (!string.IsNullOrEmpty(keyword))
            {
                endpoint += $"&keyword={keyword}";
            }

            var responseBook = await client.GetAsync(endpoint);
            var responseCategory = await client.GetAsync("/api/Category");
            var paginationData = new PaginationViewModel<BookViewModel>();

            if (responseBook.IsSuccessStatusCode)
            {
                var content = await responseBook.Content.ReadAsStringAsync();
                paginationData = JsonSerializer.Deserialize<PaginationViewModel<BookViewModel>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (responseCategory.IsSuccessStatusCode)
            {
                var content = await responseCategory.Content.ReadAsStringAsync();
                paginationData.Categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return View("Store", paginationData);
        }
        catch (Exception ex)
        {
            // Use logger instead of console for production code
            Console.WriteLine($"Filter error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }



}