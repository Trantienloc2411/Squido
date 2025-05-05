using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class StatsController(IStatsService statsService) : ControllerBase
{
    private readonly IStatsService _statsService = statsService;

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _statsService.GetStatsAsync();
        return Ok(stats);
    }
}