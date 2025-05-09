using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController(IRatingService ratingService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRatingByBookId([FromRoute(Name = "id")] string id)
    {
        try
        {
            var result = await ratingService.GetRatingsByBookId(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingViewModel rating)
    {
        try
        {
            var result = await ratingService.CreateRating(rating);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    IsSuccess = true,
                    Message = result.Message,
                    Data = result.Data
                });
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
}