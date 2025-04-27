using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using Squido.Helper;

namespace Squido.Controllers
{
    public class CheckoutMvcController(IHttpClientFactory httpClientFactory, IMapper mapper) : BaseController
    {
        public async Task<IActionResult> Index()
        {
            if (!ViewBag.IsLoggedIn)
            {
                TempData["ErrorLogin"] = "For best experience please login first";
                return RedirectToAction("Index", "AuthMvc");
            }
            else
            {

                //fetch Data user
                var userId = HttpContext.Session.GetString("Id");
                var client = httpClientFactory.CreateClient("Squido");
                var responseUser = await client.GetAsync($"/api/User/{userId}");
                var user = new UserViewModel();
                if (responseUser.IsSuccessStatusCode)
                {
                    var content = await responseUser.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<UserViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    TempData["Error"] = "Something happen unexpected. Please try again. ";
                    return View("Views/Checkout/Index.cshtml");
                }

                //Fetch Data Book in Cart
                var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? [];
                var bookList = new List<BookViewModel>();
                var responseBook = await client.GetAsync("/api/Book");
                if (responseBook.IsSuccessStatusCode)
                {
                    var content = await responseBook.Content.ReadAsStringAsync();
                    bookList = JsonSerializer.Deserialize<List<BookViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                var result = bookList!.Where(b
                    => cart.Any(c => c.BookId == b.BookId)).ToList();

                var cartItems = result.Select(item => new CartItemViewModel
                {
                    BookId = item.BookId,
                    Price = item.Price,
                    QuantityCart = cart.FirstOrDefault(c => c.BookId == item.BookId)?.Quantity ?? 0,
                    QuantityOnStore = item.Quantity,
                    Title = item.Title,
                    AuthorName = item.AuthorName
                })
                    .ToList();

                var viewModal = new CheckoutViewModel
                {
                    CartItemViewModels = cartItems,
                    UserViewModel = user
                };
                return View("Views/Checkout/Index.cshtml", viewModal);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(UserViewModel userViewModel)
        {
            try
            {
                // Make sure you have a valid user ID
                if (userViewModel.Id == null)
                {
                    TempData["Error"] = "User ID is missing.";
                    return RedirectToAction("Index");
                }

                var client = httpClientFactory.CreateClient("Squido");

                // Set authentication token
                var token = HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var updateAddressModel = mapper.Map(userViewModel, new UpdateAddressRequestVm());

                // Make sure only address-related properties are included
                // and that the ID isn't null in the request body
                updateAddressModel.Id = userViewModel.Id;

                var response = await client.PutAsJsonAsync($"/api/User", updateAddressModel);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Address successfully updated";
                    return RedirectToAction("Index");
                }

                // Get more details about the error
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error updating address: {response.StatusCode} - {errorContent}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderViewModel orderViewModel)
        {
            try
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();


                // Check if the cart is empty
                if (cart.Count == 0)
                {
                    TempData["Error"] = "Your cart is empty. Please add items to your cart before checking out.";
                    return RedirectToAction("Index");
                }
                else
                {

                }






                var client = httpClientFactory.CreateClient("Squido");

                // Set authentication token
                var token = HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.PostAsJsonAsync("/api/Order", new { orderViewModel });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Order successfully created";
                    HttpContext.Session.Remove("Cart");
                    return RedirectToAction("Index", "HomeMvc");
                }

                // Get more details about the error
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error creating order: {response.StatusCode} - {errorContent}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
