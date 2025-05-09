using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using SharedViewModal.ViewModels;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class AuthorControllerTests
{
    private readonly Mock<IAuthorService> _mockAuthorService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthorController _controller;

    public AuthorControllerTests()
    {
        _mockAuthorService = new Mock<IAuthorService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new AuthorController(_mockAuthorService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAuthors_WithValidParameters_ReturnsSuccessResponse()
    {
        // Arrange
        var keyword = "test";
        var page = 1;
        var pageSize = 10;
        var authors = new List<AuthorViewModel>
        {
            new() { Id = "1", FullName = "Author 1" },
            new() { Id = "2", FullName = "Author 2" }
        };

        _mockAuthorService
            .Setup(service => service.GetAuthors(keyword, page, pageSize))
            .ReturnsAsync(authors);

        // Act
        var result = await _controller.GetAllAuthors(keyword, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Message);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Author 1", result.Data[0].FullName);
        Assert.Equal("Author 2", result.Data[1].FullName);
    }

    [Fact]
    public async Task GetAllAuthors_WithNoResults_ReturnsEmptyList()
    {
        // Arrange
        var keyword = "nonexistent";
        var page = 1;
        var pageSize = 10;
        var emptyAuthors = new List<AuthorViewModel>();

        _mockAuthorService
            .Setup(service => service.GetAuthors(keyword, page, pageSize))
            .ReturnsAsync(emptyAuthors);

        // Act
        var result = await _controller.GetAllAuthors(keyword, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Message);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetAllAuthors_WithDefaultParameters_ReturnsSuccessResponse()
    {
        // Arrange
        var authors = new List<AuthorViewModel>
        {
            new() { AuthorId = "1", FullName = "Author 1" }
        };

        _mockAuthorService
            .Setup(service => service.GetAuthors(null, 1, 10))
            .ReturnsAsync(authors);

        // Act
        var result = await _controller.GetAllAuthors();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Message);
        Assert.Single(result.Data);
        Assert.Equal("Author 1", result.Data[0].FullName);
    }

    [Fact]
    public async Task GetAuthors_WithPagination_ReturnsPaginatedResult()
    {
        // Arrange
        var keyword = "test";
        var page = 1;
        var pageSize = 10;
        var authors = new List<AuthorViewModel>
        {
            new() { Id = "1", FullName = "Author 1" },
            new() { Id = "2", FullName = "Author 2" }
        };

        _mockAuthorService
            .Setup(service => service.GetAuthors(keyword))
            .ReturnsAsync(authors);

        _mockAuthorService
            .Setup(service => service.GetAuthors(page, pageSize, keyword))
            .ReturnsAsync(authors);

        // Act
        var result = await _controller.GetAuthors(keyword, page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PaginationViewModel<AuthorViewModel>>(okResult.Value);
        Assert.Equal(2, returnValue.Data.Count);
        Assert.Equal(1, returnValue.CurrentPage);
        Assert.Equal(1, returnValue.PageCount);
        Assert.Equal(2, returnValue.TotalCount);
    }

    [Fact]
    public async Task GetAuthors_WithoutPagination_ReturnsAllAuthors()
    {
        // Arrange
        var keyword = "test";
        var authors = new List<AuthorViewModel>
        {
            new() { Id = "1", FullName = "Author 1" },
            new() { Id = "2", FullName = "Author 2" }
        };

        _mockAuthorService
            .Setup(service => service.GetAuthors(keyword))
            .ReturnsAsync(authors);

        // Act
        var result = await _controller.GetAuthors(keyword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<AuthorViewModel>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }
} 