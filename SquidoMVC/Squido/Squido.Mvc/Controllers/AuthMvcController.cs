using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;

namespace Squido.Controllers
{
    public class AuthMvcController(IHttpClientFactory httpClientFactory) : BaseController
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("AccessToken");

            // ✅ Only set error if not already set by another controller
            if (!string.IsNullOrEmpty(token))
            {
                if (TempData["ErrorLogin"] == null)
                {
                    TempData["ErrorLogin"] = "For best experience please login first";
                }
                return View("Views/Index.cshtml");
            }

            return View("Views/Authorization/Login.cshtml");
        }


        public Task<IActionResult> RegisterPage()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                return Task.FromResult<IActionResult>(View("Views/Index.cshtml"));
            }
            return Task.FromResult<IActionResult>(View("Views/Authorization/Register.cshtml"));
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestVm model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var client = httpClientFactory.CreateClient("Squido");
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {

                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    HttpContext.Session.SetString("AccessToken", authResponse!.AccessToken);
                    HttpContext.Session.SetString("Username", authResponse!.User!.Username!);
                    HttpContext.Session.SetString("RefreshToken", authResponse!.RefreshToken!);
                    HttpContext.Session.SetString("Id", authResponse!.User!.Id!.ToString());

                    return RedirectToAction("Index", "HomeMvc");
                }
                else
                {
                    TempData["LoginError"] = response is not null ? responseBody : "Something went wrong";
                    return View("Views/Authorization/Login.cshtml");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;

            }
        }

        [HttpPost]
        public Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("AccessToken");
            HttpContext.Session.Remove("RefreshToken");
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("Cart");
            return Task.FromResult<IActionResult>(RedirectToAction("Index", "HomeMvc"));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestVm model)
        {
            try
            {
                model.RoleId = 1;
                var client = httpClientFactory.CreateClient("Squido");
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Auth/register", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["RegisterSuccess"] = "Account successfully registered";
                    return RedirectToAction("Index", "AuthMvc");
                }
                else
                {
                    TempData["RegisterError"] = responseBody is not null ? responseBody : "Something went wrong";
                    return RedirectToAction("RegisterPage", "AuthMvc");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
