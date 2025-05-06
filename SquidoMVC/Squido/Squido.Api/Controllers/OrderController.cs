using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Helper;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderViewModel model)
        {
            try
            {
                var result = await orderService.CreateOrderAsync(model.OrderViewModel, model.OrderItemViewModels);
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

        [HttpGet("/orderId/{id}")]
        public async Task<IActionResult> GetOrderByOrderId([FromRoute (Name = "id")]string orderId)
        {
            try
            {
                var result = await orderService.GetOrderAsync(orderId);
                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        isSuccess = result.IsSuccess,
                        result.Data,

                    });
                }

                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("/userId/{id}")]
        public async Task<IActionResult> GetOrderByUserId([FromRoute (Name = "id")] Guid userId)
        {
            try
            {
                var result = await orderService.GetOrderByUserIdAsync(userId);
                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        IsSuccess = result.IsSuccess,
                        result.Data,
                    });
                }
                return BadRequest(result.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrder(string? keyword, int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await orderService.GetAllOrdersAsync(keyword);
                if (result.IsSuccess)
                {
                    var allOrders = result.Data;

                    // Apply pagination
                    var pagedOrders = allOrders
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    // Prepare response with pagination metadata
                    var response = new
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = allOrders.Count,
                        TotalPages = (int)Math.Ceiling((double)allOrders.Count / pageSize),
                        Items = pagedOrders
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        
        
    }
}
