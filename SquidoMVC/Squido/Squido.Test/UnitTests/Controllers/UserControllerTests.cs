using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using SharedViewModal.ViewModels;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new UserController(_mockUserService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Get_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserViewModel
        {
            Id = userId.ToString(),
            Username = "testuser",
            Email = "test@example.com"
        };

        var response = new ResponseMessage<UserViewModel>
        {
            IsSuccess = true,
            Message = "User found",
            Data = user
        };

        _mockUserService
            .Setup(service => service.GetUserByIdAsync(userId))
            .ReturnsAsync(response);

        _mockMapper
            .Setup(m => m.Map<UserViewModel>(It.IsAny<object>()))
            .Returns(user);

        // Act
        var result = await _controller.Get(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserViewModel>(okResult.Value);
        Assert.Equal(user.Id, returnValue.Id);
        Assert.Equal(user.Username, returnValue.Username);
        Assert.Equal(user.Email, returnValue.Email);
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var response = new ResponseMessage<UserViewModel>
        {
            IsSuccess = false,
            Message = "User not found",
            Data = null
        };

        _mockUserService
            .Setup(service => service.GetUserByIdAsync(userId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Get(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("User not found", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserViewModel
        {
            Id = userId.ToString(),
            Username = "updateduser",
            Email = "updated@example.com"
        };

        var response = new ResponseMessage<UserViewModel>
        {
            IsSuccess = true,
            Message = "User updated successfully",
            Data = user
        };

        _mockUserService
            .Setup(service => service.UpdateUserAsync(user, userId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.UpdateUser(user);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ResponseMessage<UserViewModel>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
        Assert.Equal("User updated successfully", returnValue.Message);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserViewModel
        {
            Id = userId.ToString(),
            Username = "updateduser",
            Email = "invalid-email"
        };

        var response = new ResponseMessage<UserViewModel>
        {
            IsSuccess = false,
            Message = "Invalid email format",
            Data = null
        };

        _mockUserService
            .Setup(service => service.UpdateUserAsync(user, userId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.UpdateUser(user);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid email format", badRequestResult.Value);
    }
} 