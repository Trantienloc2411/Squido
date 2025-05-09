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
        var books = new List<BookViewModel>
        {
            new() { Id = "1", Title = "Book 1" },
            new() { Id = "2", Title = "Book 2" }
        };

        var categories = new List<CategoryViewModel>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };

        _mockStatsService
            .Setup(service => service.GetTopBooks())
            .ReturnsAsync(books);

        _mockStatsService
            .Setup(service => service.GetTopCategories())
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StatsViewModel>(okResult.Value);
        Assert.Equal(2, returnValue.TopBooks.Count());
        Assert.Equal(2, returnValue.TopCategories.Count());
    }

    [Fact]
    public async Task GetStats_WithEmptyData_ReturnsEmptyStats()
    {
        // Arrange
        var emptyBooks = new List<BookViewModel>();
        var emptyCategories = new List<CategoryViewModel>();

        _mockStatsService
            .Setup(service => service.GetTopBooks())
            .ReturnsAsync(emptyBooks);

        _mockStatsService
            .Setup(service => service.GetTopCategories())
            .ReturnsAsync(emptyCategories);

        // Act
        var result = await _controller.GetStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StatsViewModel>(okResult.Value);
        Assert.Empty(returnValue.TopBooks);
        Assert.Empty(returnValue.TopCategories);
    }
} 