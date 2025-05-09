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
    public async Task GetStats_ReturnsOkResult_WithStats()
    {
        // Arrange
        var expectedStats = new StatViewModel
        {
            TotalBooks = 100,
            TotalCategories = 10,
            TotalCustomers = 50,
            TotalRevenues = 5000,
            TopBooks = new List<BookViewModel>
            {
                new() { BookId = "1", Title = "Book 1" },
                new() { BookId = "2", Title = "Book 2" }
            },
            TopCategories = new List<CategoryViewModel>
            {
                new() { CategoryId = 1, Name = "Category 1" },
                new() { CategoryId = 2, Name = "Category 2" }
            }
        };

        _mockStatsService
            .Setup(service => service.GetStatsAsync())
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StatViewModel>(okResult.Value);
        Assert.Equal(100, returnValue.TotalBooks);
        Assert.Equal(10, returnValue.TotalCategories);
        Assert.Equal(50, returnValue.TotalCustomers);
        Assert.Equal(5000, returnValue.TotalRevenues);
        Assert.Equal(2, returnValue.TopBooks.Count);
        Assert.Equal(2, returnValue.TopCategories.Count);
    }

    [Fact]
    public async Task GetStats_WithZeroValues_ReturnsOkResult()
    {
        // Arrange
        var expectedStats = new StatViewModel
        {
            TotalBooks = 0,
            TotalCategories = 0,
            TotalCustomers = 0,
            TotalRevenues = 0,
            TopBooks = new List<BookViewModel>(),
            TopCategories = new List<CategoryViewModel>()
        };

        _mockStatsService
            .Setup(service => service.GetStatsAsync())
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StatViewModel>(okResult.Value);
        Assert.Equal(0, returnValue.TotalBooks);
        Assert.Equal(0, returnValue.TotalCategories);
        Assert.Equal(0, returnValue.TotalCustomers);
        Assert.Equal(0, returnValue.TotalRevenues);
        Assert.Empty(returnValue.TopBooks);
        Assert.Empty(returnValue.TopCategories);
    }
} 