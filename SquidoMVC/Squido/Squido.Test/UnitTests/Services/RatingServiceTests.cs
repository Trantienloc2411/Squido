using AutoMapper;
using Moq;
using SharedViewModal.ViewModels;
using System;
using System.Collections.Generic;
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

        // Act
        var result = await _ratingService.CreateRating(createRatingViewModel);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Added Rating", result.Message);
        Assert.NotNull(result.Data);
        _mockUnitOfWork.Verify(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
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

        _mockUnitOfWork.Setup(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()))
            .ThrowsAsync(new Exception("Invalid rating value"));

        // Act
        var result = await _ratingService.CreateRating(createRatingViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid rating value", result.Message);
        _mockUnitOfWork.Verify(u => u.RatingRepository.AddAsync(It.IsAny<Rating>()), Times.Once);
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
}
