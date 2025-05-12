using AutoMapper;
using Moq;
using SharedViewModal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Helper;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Services;
using Xunit;

namespace Squido.Test.UnitTests.Services;

public class RatingServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RatingService _ratingService;

    public RatingServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _ratingService = new RatingService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateRating_ValidData_ReturnsSuccess()
    {
        // Arrange
        var ratingViewModel = new CreateRatingViewModel
        {
            BookId = "test-book-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Great book!"
        };

        var rating = new Rating
        {
            BookId = ratingViewModel.BookId,
            CustomerId = ratingViewModel.CustomerId,
            RatingValue = ratingViewModel.RatingValue,
            Comment = ratingViewModel.Comment
        };

        _mockMapper.Setup(m => m.Map<Rating>(ratingViewModel))
            .Returns(rating);

        _mockUnitOfWork.Setup(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ratingService.CreateRating(ratingViewModel);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Rating created successfully", result.Message);
        Assert.Equal(ratingViewModel, result.Data);
    }

    [Fact]
    public async Task CreateRating_NullModel_ReturnsFalse()
    {
        // Arrange
        CreateRatingViewModel ratingViewModel = null;

        // Act
        var result = await _ratingService.CreateRating(ratingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid rating data", result.Message);
        Assert.Null(result.Data);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateRating_InvalidRatingValue_ReturnsFalse(int invalidRating)
    {
        // Arrange
        var ratingViewModel = new CreateRatingViewModel
        {
            BookId = "test-book-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = invalidRating,
            Comment = "Test comment"
        };

        // Act
        var result = await _ratingService.CreateRating(ratingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid rating value", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task CreateRating_MappingFails_ReturnsFalse()
    {
        // Arrange
        var ratingViewModel = new CreateRatingViewModel
        {
            BookId = "test-book-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Test comment"
        };

        _mockMapper.Setup(m => m.Map<Rating>(ratingViewModel))
            .Returns((Rating)null);

        // Act
        var result = await _ratingService.CreateRating(ratingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to map rating data", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task CreateRating_DatabaseError_ReturnsFalse()
    {
        // Arrange
        var ratingViewModel = new CreateRatingViewModel
        {
            BookId = "test-book-id",
            CustomerId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Test comment"
        };

        var rating = new Rating
        {
            BookId = ratingViewModel.BookId,
            CustomerId = ratingViewModel.CustomerId,
            RatingValue = ratingViewModel.RatingValue,
            Comment = ratingViewModel.Comment
        };

        _mockMapper.Setup(m => m.Map<Rating>(ratingViewModel))
            .Returns(rating);

        _mockUnitOfWork.Setup(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()))
            .ThrowsAsync(new NullReferenceException("Object reference not set to an instance of an object"));

        // Act
        var result = await _ratingService.CreateRating(ratingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Object reference not set to an instance of an object", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetRatingsByBookId_ValidId_ReturnsRatings()
    {
        // Arrange
        var bookId = "test-book-id";
        var customerId = Guid.NewGuid();
        var createdDate = DateTime.Now;
        var ratings = new List<Rating>
        {
            new Rating { Id = "1", BookId = bookId, CustomerId = customerId, RatingValue = 5, CreatedDate = createdDate },
            new Rating { Id = "2", BookId = bookId, CustomerId = customerId, RatingValue = 4, CreatedDate = createdDate }
        };

        var ratingViewModels = new List<RatingViewModel>
        {
            new RatingViewModel { Id = "1", BookId = bookId, CustomerId = customerId, RatingValue = 5, CreatedDate = createdDate },
            new RatingViewModel { Id = "2", BookId = bookId, CustomerId = customerId, RatingValue = 4, CreatedDate = createdDate }
        };

        _mockUnitOfWork.Setup(u => u.RatingRepository.GetAllWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, object>>>()))
            .ReturnsAsync(ratings);

        _mockMapper.Setup(m => m.Map<List<RatingViewModel>>(ratings))
            .Returns(ratingViewModels);

        // Act
        var result = await _ratingService.GetRatingsByBookId(bookId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Ratings retrieved successfully", result.Message);
        Assert.Equal(2, result.Data.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetRatingsByBookId_InvalidId_ReturnsFalse(string invalidBookId)
    {
        // Act
        var result = await _ratingService.GetRatingsByBookId(invalidBookId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid book ID", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetRatingsByBookId_DatabaseError_ReturnsFalse()
    {
        // Arrange
        var bookId = "test-book-id";
        _mockUnitOfWork.Setup(u => u.RatingRepository.GetAllWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, object>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _ratingService.GetRatingsByBookId(bookId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Database error", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetRatingsByUserId_ValidId_ReturnsRatings()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createdDate = DateTime.Now;
        var ratings = new List<Rating>
        {
            new Rating { Id = "1", BookId = "book1", CustomerId = userId, RatingValue = 5, CreatedDate = createdDate },
            new Rating { Id = "2", BookId = "book2", CustomerId = userId, RatingValue = 4, CreatedDate = createdDate }
        };

        var ratingViewModels = new List<RatingViewModel>
        {
            new RatingViewModel { Id = "1", BookId = "book1", CustomerId = userId, RatingValue = 5, CreatedDate = createdDate },
            new RatingViewModel { Id = "2", BookId = "book2", CustomerId = userId, RatingValue = 4, CreatedDate = createdDate }
        };

        _mockUnitOfWork.Setup(u => u.RatingRepository.GetAllWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, object>>>()))
            .ReturnsAsync(ratings);

        _mockMapper.Setup(m => m.Map<List<RatingViewModel>>(ratings))
            .Returns(ratingViewModels);

        // Act
        var result = await _ratingService.GetRatingsByUserId(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Ratings retrieved successfully", result.Message);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetRatingsByUserId_EmptyGuid_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await _ratingService.GetRatingsByUserId(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid user ID", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetRatingsByUserId_DatabaseError_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUnitOfWork.Setup(u => u.RatingRepository.GetAllWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, object>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _ratingService.GetRatingsByUserId(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Database error", result.Message);
        Assert.Null(result.Data);
    }
}
