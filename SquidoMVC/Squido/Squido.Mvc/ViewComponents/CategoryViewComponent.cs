using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;

namespace Squido.ViewComponents;

public class CategoryViewComponent(IHttpClientFactory httpClientFactory) : ViewComponent
{ 
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("Squido");
            var response = await client.GetAsync("/api/Category");

            if (!response.IsSuccessStatusCode) return View(new HomeViewModel());
            var content = await response.Content.ReadAsStringAsync();
                
            List<CategoryViewModel>? categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            if (categories == null) return View(new HomeViewModel());
            var homeViewModel = new HomeViewModel
            {
                Categories = categories
            };
            return View(homeViewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}