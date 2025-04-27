using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromQuery] OrderViewModel orderViewModel, List<OrderItemViewModel> orderItemViewModels)
        {
            try
            {
                var result = await orderService.CreateOrderAsync(orderViewModel, orderItemViewModels);
                if (result.IsSuccess)
                {
                    return Ok(result.Data);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
