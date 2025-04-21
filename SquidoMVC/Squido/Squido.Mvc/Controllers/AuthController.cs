using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Mvc;

namespace Squido.Controllers
{
    public class AuthController(IHttpClientFactory httpClientFactory) : Controller
    {
        public async Task<IActionResult> Login()
        {
            try
            {
                var client = httpClientFactory.CreateClient("Squido");
                var response = await client.GetAsync("/api/Auth/login");
                
                if(response.IsSuccessStatusCode) 
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok();
                }
                return Ok();
            }
            catch(Exception e) {
                Console.WriteLine(e);
                throw;
                
            }
        }

    }
}
