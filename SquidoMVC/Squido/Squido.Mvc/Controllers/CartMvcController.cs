using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using Squido.Helper;

namespace Squido.Controllers;

public class CartMvcController(IHttpClientFactory clientFactory) : BaseController
{
    [HttpPost]
    public IActionResult AddToCart(string bookId, int quantity)
    {
        try
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => string.Equals(c.Id, bookId, StringComparison.OrdinalIgnoreCase));
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem() { Id = bookId, Quantity = quantity });
            }
            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("LoadCartItem", "CartMvc");
        }
        catch (System.Exception)
        {

            throw;
        }
    }


    public async Task<IActionResult> LoadCartItem()
    {
        try
        {
            var client = clientFactory.CreateClient("Squido");
            //List from cart
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            var response = await client.GetAsync($"api/Book");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bookList = JsonSerializer.Deserialize<List<BookViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                //List after filtering from cart
                var result = bookList!.Where(b
                    => cart.Any(c => c.Id == b.Id)).ToList();

                var cartItems = result.Select(item => new CartItemViewModel
                {
                    Id = item.Id,
                    Price = item.Price,
                    QuantityCart = cart.FirstOrDefault(c => c.Id == item.Id)?.Quantity ?? 0,
                    QuantityOnStore = item.Quantity,
                    Title = item.Title,
                    AuthorName = item.AuthorName
                })
                    .ToList();
                return View("Index", cartItems);
            }
            else
            {
                return View("Error");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateCartItem(string bookId, int quantity)
    {
        try
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => string.Equals(c.Id, bookId, StringComparison.OrdinalIgnoreCase));
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("LoadCartItem", "CartMvc");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost]
    public IActionResult DeleteOneFromCart(string bookId)
    {
        try
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            cart.RemoveAll(c => string.Equals(c.Id, bookId, StringComparison.OrdinalIgnoreCase));

            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("LoadCartItem", "CartMvc");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost]
    public IActionResult ClearCart()
    {
        try
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("LoadCartItem", "CartMvc");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }



}