using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using Squido.Models.Entities;
using Squido.Models.ViewModals;

namespace Squido.Controllers;

public class BookMvcController(IHttpClientFactory clientFactory) : BaseController
{
    

    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var client = clientFactory.CreateClient("Squido");
            var responseBook = await client.GetAsync($"/api/Book/{id}");
            
            if (responseBook.IsSuccessStatusCode)
            {
                var content  = await responseBook.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<ViewBookDetailViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(book);
            }
            return View();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}