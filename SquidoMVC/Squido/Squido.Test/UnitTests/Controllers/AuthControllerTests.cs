using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Services;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockJwtService = new Mock<IJwtService>();
        _mockUserService = new Mock<IUserService>();
        _controller = new AuthController(_mockJwtService.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new UserViewModel
        {
            Id = Guid.NewGuid().ToString(),
            Username = "testuser",
            Email = "test@example.com",
            IsDeleted = false,
            Role = new RoleViewModel { RoleId = 1 }
        };

        _mockUserService
            .Setup(service => service.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(user);

        _mockJwtService
            .Setup(service => service.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns("access_token");

        _mockJwtService
            .Setup(service => service.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(new RefreshToken { Token = "refresh_token" });

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = okResult.Value as dynamic;
        Assert.NotNull(returnValue);
        Assert.Equal("access_token", returnValue.AccessToken.ToString());
        Assert.Equal("refresh_token", returnValue.RefreshToken.ToString());
        Assert.Equal(user.Id, returnValue.User.Id.ToString());
        Assert.Equal(user.Username, returnValue.User.Username.ToString());
        Assert.Equal(user.Email, returnValue.User.Email.ToString());
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var user = new UserViewModel
        {
            Email = null
        };

        _mockUserService
            .Setup(service => service.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email or password is incorrect", badRequestResult.Value);
    }

    [Fact]
    public async Task Login_WithDeletedUser_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new UserViewModel
        {
            Id = Guid.NewGuid().ToString(),
            Username = "testuser",
            Email = "test@example.com",
            IsDeleted = true,
            Role = new RoleViewModel { RoleId = 1 }
        };

        _mockUserService
            .Setup(service => service.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("This account was deleted or not authorized.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var registerRequest = new RegisterRequestVm
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123"
        };

        var response = new ResponseMessage<RegisterRequestVm>
        {
            IsSuccess = true,
            Message = "Registration successful",
            Data = registerRequest
        };

        _mockUserService
            .Setup(service => service.CreateUserAsync(registerRequest))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(registerRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ResponseMessage<RegisterRequestVm>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
        Assert.Equal("Registration successful", returnValue.Message);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequestVm
        {
            Username = "newuser",
            Email = "invalid-email",
            Password = "password123"
        };

        var response = new ResponseMessage<RegisterRequestVm>
        {
            IsSuccess = false,
            Message = "Invalid email format",
            Data = null
        };

        _mockUserService
            .Setup(service => service.CreateUserAsync(registerRequest))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(registerRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid email format", badRequestResult.Value);
    }
} 