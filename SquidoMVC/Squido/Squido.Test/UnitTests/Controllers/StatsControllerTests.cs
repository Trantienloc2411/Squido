using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using SharedViewModal.ViewModels;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class StatsControllerTests
{
    private readonly Mock<IStatsService> _mockStatsService;
    private readonly StatsController _controller;

    public StatsControllerTests()
    {
        _mockStatsService = new Mock<IStatsService>();
        _controller = new StatsController(_mockStatsService.Object);
    }

    [Fact]
    public async Task GetStats_ReturnsStatsViewModel()
    {
        // Arrange
        var stats = new StatViewModel
        {
            TopBooks = new List<BookViewModel>
            {
                new() { Id = "1", Title = "Book 1" },
                new() { Id = "2", Title = "Book 2" }
            },
            TopCategories = new List<CategoryViewModel>
            {
                new() { Id = 1, Name = "Category 1" },
                new() { Id = 2, Name = "Category 2" }
            }
        };

        _mockStatsService
            .Setup(service => service.GetStatsAsync())
            .ReturnsAsync(stats);

        // Act
        var result = await _controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StatViewModel>(okResult.Value);
        Assert.Equal(2, returnValue.TopBooks?.Count ?? 0);
        Assert.Equal(2, returnValue.TopCategories?.Count ?? 0);
    }

    [Fact]
    public async Task GetStats_WithEmptyData_ReturnsEmptyStats()
    {
        // Arrange
        var stats = new StatViewModel
        {
            TopBooks = new List<BookViewModel>(),
            TopCategories = new List<CategoryViewModel>()
        };

        _mockStatsService
            .Setup(service => service.GetStatsAsync())
            .ReturnsAsync(stats);

        // Act
        var result = await _controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StatViewModel>(okResult.Value);
        Assert.Empty(returnValue.TopBooks ?? new List<BookViewModel>());
        Assert.Empty(returnValue.TopCategories ?? new List<CategoryViewModel>());
    }
} 