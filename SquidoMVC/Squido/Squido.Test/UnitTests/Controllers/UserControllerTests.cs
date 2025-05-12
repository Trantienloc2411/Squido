using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedViewModal.ViewModels;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _userController = new UserController(_mockUserService.Object, _mockMapper.Object);
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
            Email = "test@example.com",
            IsDeleted = false,
            Role = new RoleViewModel { Id = 1 }
        };

        var response = new ResponseMessage<UserViewModel>
        {
            IsSuccess = true,
            Message = "User found",
            Data = user
        };

        _mockUserService
            .Setup(service => service.GetUserByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(response));

        _mockMapper
            .Setup(m => m.Map<UserViewModel>(It.IsAny<object>()))
            .Returns(user);

        // Act
        var result = await _userController.Get(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserViewModel>(okResult.Value);
        Assert.Equal(userId.ToString(), returnValue.Id);
        Assert.Equal("testuser", returnValue.Username);
        Assert.Equal("test@example.com", returnValue.Email);
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
            .Setup(service => service.GetUserByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(response));

        // Act
        var result = await _userController.Get(userId);

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
        var result = await _userController.UpdateUser(user);

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
        var result = await _userController.UpdateUser(user);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid email format", badRequestResult.Value);
    }

    [Fact]
    public async Task GetAll_WithoutKeyword_ReturnsOkResult()
    {
        // Arrange
        var users = new List<UserViewModel>
        {
            new() { Id = Guid.NewGuid().ToString(), Username = "user1", Email = "user1@example.com" },
            new() { Id = Guid.NewGuid().ToString(), Username = "user2", Email = "user2@example.com" }
        };
        var response = new ResponseMessage<List<UserViewModel>>
        {
            IsSuccess = true,
            Message = "Users retrieved successfully",
            Data = users
        };
        _mockUserService
            .Setup(service => service.GetAllUser(null))
            .ReturnsAsync(response);
        // Act
        var result = await _userController.GetAll(null);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = okResult.Value;
        var type = returnValue.GetType();
        var records = (IEnumerable<UserViewModel>)type.GetProperty("records")!.GetValue(returnValue)!;
        var currentPage = (int)type.GetProperty("currentPage")!.GetValue(returnValue)!;
        var pageSize = (int)type.GetProperty("pageSize")!.GetValue(returnValue)!;
        var totalRecords = (int)type.GetProperty("totalRecords")!.GetValue(returnValue)!;
        Assert.Equal(2, records.Count());
        Assert.Equal(1, currentPage);
        Assert.Equal(10, pageSize);
        Assert.Equal(2, totalRecords);
    }

    [Fact]
    public async Task GetAll_WithKeyword_ReturnsFilteredResults()
    {
        // Arrange
        var keyword = "user1";
        var users = new List<UserViewModel>
        {
            new() { Id = Guid.NewGuid().ToString(), Username = "user1", Email = "user1@example.com" }
        };
        var response = new ResponseMessage<List<UserViewModel>>
        {
            IsSuccess = true,
            Message = "Users retrieved successfully",
            Data = users
        };
        _mockUserService
            .Setup(service => service.GetAllUser(keyword))
            .ReturnsAsync(response);
        // Act
        var result = await _userController.GetAll(keyword);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = okResult.Value;
        var type = returnValue.GetType();
        var records = (IEnumerable<UserViewModel>)type.GetProperty("records")!.GetValue(returnValue)!;
        var currentPage = (int)type.GetProperty("currentPage")!.GetValue(returnValue)!;
        var pageSize = (int)type.GetProperty("pageSize")!.GetValue(returnValue)!;
        var totalRecords = (int)type.GetProperty("totalRecords")!.GetValue(returnValue)!;
        Assert.Single(records);
        Assert.Equal("user1", records.First().Username);
        Assert.Equal(1, currentPage);
        Assert.Equal(10, pageSize);
        Assert.Equal(1, totalRecords);
    }

    [Fact]
    public async Task GetAll_WithError_ReturnsBadRequest()
    {
        // Arrange
        var response = new ResponseMessage<List<UserViewModel>>
        {
            IsSuccess = false,
            Message = "Failed to retrieve users",
            Data = null
        };
        _mockUserService
            .Setup(service => service.GetAllUser(null))
            .ReturnsAsync(response);
        // Act
        var result = await _userController.GetAll(null);
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = badRequestResult.Value;
        var type = errorResponse.GetType();
        var message = (string)type.GetProperty("message")!.GetValue(errorResponse)!;
        Assert.Equal("Failed to retrieve users.", message);
    }

    [Fact]
    public async Task Get_WhenServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserService
            .Setup(service => service.GetUserByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userController.Get(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Database error", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateUser_WhenServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var user = new UserViewModel
        {
            Id = Guid.NewGuid().ToString(),
            Username = "user",
            Email = "user@example.com"
        };

        _mockUserService
            .Setup(service => service.UpdateUserAsync(It.IsAny<UserViewModel>(), It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Update failed"));

        // Act
        var result = await _userController.UpdateUser(user);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Update failed", badRequestResult.Value);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsPagedResult()
    {
        // Arrange
        var users = Enumerable.Range(1, 25).Select(i => new UserViewModel
        {
            Id = Guid.NewGuid().ToString(),
            Username = $"user{i}",
            Email = $"user{i}@example.com"
        }).ToList();

        var response = new ResponseMessage<List<UserViewModel>>
        {
            IsSuccess = true,
            Message = "Users retrieved successfully",
            Data = users
        };

        _mockUserService
            .Setup(service => service.GetAllUser(null))
            .ReturnsAsync(response);

        // Act
        var result = await _userController.GetAll(null, 2, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = okResult.Value;
        var type = returnValue.GetType();
        var records = (IEnumerable<UserViewModel>)type.GetProperty("records")!.GetValue(returnValue)!;
        var currentPage = (int)type.GetProperty("currentPage")!.GetValue(returnValue)!;
        var pageSize = (int)type.GetProperty("pageSize")!.GetValue(returnValue)!;
        var totalRecords = (int)type.GetProperty("totalRecords")!.GetValue(returnValue)!;
        Assert.Equal(10, records.Count());
        Assert.Equal(2, currentPage);
        Assert.Equal(10, pageSize);
        Assert.Equal(25, totalRecords);
    }

    [Fact]
    public async Task GetAll_WhenServiceReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var response = new ResponseMessage<List<UserViewModel>>
        {
            IsSuccess = false,
            Message = "Failed to retrieve users",
            Data = null
        };
        _mockUserService.Setup(x => x.GetAllUser(null))
            .ReturnsAsync(response);

        // Act
        var result = await _userController.GetAll(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = badRequestResult.Value;
        var type = errorResponse.GetType();
        var message = (string)type.GetProperty("message")!.GetValue(errorResponse)!;
        Assert.Equal("Failed to retrieve users.", message);
    }
} 