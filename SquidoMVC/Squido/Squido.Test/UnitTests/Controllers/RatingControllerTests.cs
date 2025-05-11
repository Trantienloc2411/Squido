using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedViewModal.ViewModels;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using WebApplication1.Helper;
using System.Text.Json;

namespace Squido.Test.UnitTests.Controllers;

public class RatingControllerTests
{
    private readonly Mock<IRatingService> _mockRatingService;
    private readonly RatingController _controller;

    public RatingControllerTests()
    {
        _mockRatingService = new Mock<IRatingService>();
        _controller = new RatingController(_mockRatingService.Object);
    }

    [Fact]
    public async Task GetRatingByBookId_ValidId_ReturnsOkResult()
    {
        // Arrange
        var bookId = "test-book-id";
        var expectedResponse = new ResponseMessage<List<RatingViewModel>>
        {
            IsSuccess = true,
            Data = new List<RatingViewModel>(),
            Message = "Success"
        };
        _mockRatingService.Setup(x => x.GetRatingsByBookId(bookId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetRatingByBookId(bookId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ResponseMessage<List<RatingViewModel>>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
    }

    [Fact]
    public async Task GetRatingByBookId_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var bookId = "invalid-id";
        var expectedResponse = new ResponseMessage<List<RatingViewModel>>
        {
            IsSuccess = false,
            Message = "Book not found"
        };
        _mockRatingService.Setup(x => x.GetRatingsByBookId(bookId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetRatingByBookId(bookId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Book not found", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateRating_ValidData_ReturnsOkResult()
    {
        // Arrange
        var rating = new CreateRatingViewModel
        {
            BookId = "test-book-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Great book!"
        };
        var expectedResponse = new ResponseMessage<CreateRatingViewModel>
        {
            IsSuccess = true,
            Data = rating,
            Message = "Rating created successfully"
        };
        _mockRatingService.Setup(x => x.CreateRating(rating))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateRating(rating);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(okResult.Value);
        var response = JsonSerializer.Deserialize<ResponseMessage<CreateRatingViewModel>>(json);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(expectedResponse.Message, response.Message);
        Assert.Equal(JsonSerializer.Serialize(rating), JsonSerializer.Serialize(response.Data));
    }

    [Fact]
    public async Task CreateRating_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var rating = new CreateRatingViewModel
        {
            BookId = "invalid-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = 6, // Invalid rating
            Comment = "Test comment"
        };
        var expectedResponse = new ResponseMessage<CreateRatingViewModel>
        {
            IsSuccess = false,
            Message = "Invalid rating value"
        };
        _mockRatingService.Setup(x => x.CreateRating(rating))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateRating(rating);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid rating value", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateRating_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var rating = new CreateRatingViewModel
        {
            BookId = "test-book-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Test comment"
        };
        _mockRatingService.Setup(x => x.CreateRating(rating))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateRating(rating);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Database error", statusCodeResult.Value);
    }
} 