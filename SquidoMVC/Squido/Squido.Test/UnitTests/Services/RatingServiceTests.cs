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
    public async Task CreateRating_Success_ReturnsSuccessResponse()
    {
        // Arrange
        var createRatingViewModel = new CreateRatingViewModel
        {
            CustomerId = Guid.NewGuid(),
            BookId = "123",
            RatingValue = 5,
            Comment = "Great book!"
        };

        var rating = new Rating
        {
            Id = "test-id",
            CustomerId = createRatingViewModel.CustomerId,
            BookId = createRatingViewModel.BookId,
            RatingValue = createRatingViewModel.RatingValue,
            Comment = createRatingViewModel.Comment,
            CreatedDate = DateTime.Now
        };

        _mockMapper.Setup(m => m.Map<Rating>(createRatingViewModel))
            .Returns(rating);
        _mockMapper.Setup(m => m.Map<CreateRatingViewModel>(rating))
            .Returns(createRatingViewModel);
        _mockMapper.Setup(m => m.Map<RatingViewModel>(rating))
            .Returns(new RatingViewModel 
            { 
                Id = rating.Id,
                CustomerId = rating.CustomerId,
                BookId = rating.BookId,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                CreatedDate = rating.CreatedDate
            });

        _mockUnitOfWork.Setup(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ratingService.CreateRating(createRatingViewModel);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Rating created successfully", result.Message);
        Assert.NotNull(result.Data);
        _mockUnitOfWork.Verify(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateRating_Failed_ReturnsFailedResponse()
    {
        // Arrange
        var createRatingViewModel = new CreateRatingViewModel
        {
            CustomerId = Guid.NewGuid(),
            BookId = "123",
            RatingValue = 6, // Invalid rating value
            Comment = "Great book!"
        };

        // Act
        var result = await _ratingService.CreateRating(createRatingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid rating value", result.Message);
        Assert.Null(result.Data);
        _mockUnitOfWork.Verify(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public async Task CreateRating_Exception_ReturnsExceptionResponse()
    {
        // Arrange
        var createRatingViewModel = new CreateRatingViewModel
        {
            CustomerId = Guid.NewGuid(),
            BookId = "123",
            RatingValue = 5,
            Comment = "Great book!"
        };

        _mockMapper.Setup(m => m.Map<Rating>(createRatingViewModel))
            .Throws(new Exception("Mapping failed"));

        // Act
        var result = await _ratingService.CreateRating(createRatingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Mapping failed", result.Message);
        _mockUnitOfWork.Verify(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public async Task GetRatingsByBookId_Success_ReturnsRatings()
    {
        // Arrange
        var bookId = "123";
        var ratings = new List<Rating>
        {
            new()
            {
                Id = "1",
                CustomerId = Guid.NewGuid(),
                BookId = bookId,
                RatingValue = 5,
                Comment = "Great book!",
                CreatedDate = DateTime.Now
            }
        };

        var ratingViewModels = new List<RatingViewModel>
        {
            new()
            {
                Id = "1",
                CustomerId = ratings[0].CustomerId,
                BookId = bookId,
                RatingValue = 5,
                Comment = "Great book!",
                CreatedDate = ratings[0].CreatedDate
            }
        };

        _mockUnitOfWork.Setup(u => u.RatingRepository.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Rating, bool>>>(),
                It.IsAny<Expression<Func<Rating, object>>>()))
            .ReturnsAsync(ratings);

        _mockMapper.Setup(m => m.Map<List<RatingViewModel>>(ratings))
            .Returns(ratingViewModels);

        // Act
        var result = await _ratingService.GetRatingsByBookId(bookId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Ratings retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal(bookId, result.Data[0].BookId);
    }

    [Fact]
    public async Task GetRatingsByBookId_InvalidBookId_ReturnsError()
    {
        // Arrange
        string bookId = null;

        // Act
        var result = await _ratingService.GetRatingsByBookId(bookId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid book ID", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetRatingsByUserId_Success_ReturnsRatings()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ratings = new List<Rating>
        {
            new()
            {
                Id = "1",
                CustomerId = userId,
                BookId = "123",
                RatingValue = 5,
                Comment = "Great book!",
                CreatedDate = DateTime.Now
            }
        };

        var ratingViewModels = new List<RatingViewModel>
        {
            new()
            {
                Id = "1",
                CustomerId = userId,
                BookId = "123",
                RatingValue = 5,
                Comment = "Great book!",
                CreatedDate = ratings[0].CreatedDate
            }
        };

        _mockUnitOfWork.Setup(u => u.RatingRepository.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Rating, bool>>>(),
                It.IsAny<Expression<Func<Rating, object>>>()))
            .ReturnsAsync(ratings);

        _mockMapper.Setup(m => m.Map<List<RatingViewModel>>(ratings))
            .Returns(ratingViewModels);

        // Act
        var result = await _ratingService.GetRatingsByUserId(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Ratings retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal(userId, result.Data[0].CustomerId);
    }

    [Fact]
    public async Task GetRatingsByUserId_InvalidUserId_ReturnsError()
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
}
