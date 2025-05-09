using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using SharedViewModal.ViewModels;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockCategoryService.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithCategories()
    {
        // Arrange
        var expectedCategories = new List<CategoryViewModel>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };

        _mockCategoryService
            .Setup(service => service.GetCategories())
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
        Assert.Equal("Category 1", returnValue.First().Name);
        Assert.Equal("Category 2", returnValue.Last().Name);
    }

    [Fact]
    public async Task Get_ReturnsEmptyList_WhenNoCategories()
    {
        // Arrange
        var emptyCategories = new List<CategoryViewModel>();
        _mockCategoryService
            .Setup(service => service.GetCategories())
            .ReturnsAsync(emptyCategories);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(okResult.Value);
        Assert.Empty(returnValue);
    }
} 