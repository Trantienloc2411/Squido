using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using SharedViewModal.ViewModels;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderController(_mockOrderService.Object);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var order = new OrderViewModel
        {
            CustomerId = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            PaymentMethod = PaymentMethodEnum.CreditCard,
            Status = OrderStatusEnum.Pending,
            OrderNote = "Test order",
            ShippingFee = 0
        };

        var orderItems = new List<OrderItemViewModel>
        {
            new()
            {
                BookId = Guid.NewGuid().ToString(),
                Title = "Test Book",
                AuthorName = "Test Author",
                CategoryName = "Test Category",
                Quantity = 1,
                UnitPrice = 100.00m
            }
        };

        var createOrderModel = new CreateOrderViewModel
        {
            OrderViewModel = order,
            OrderItemViewModels = orderItems
        };

        var response = new ResponseMessage<OrderResultViewModel>
        {
            IsSuccess = true,
            Message = "Order created successfully",
            Data = new OrderResultViewModel
            {
                Id = Guid.NewGuid().ToString(),
                OrderDate = order.OrderDate,
                Status = OrderStatusEnum.Pending,
                PaymentMethod = (PaymentMethod)order.PaymentMethod,
                OrderNote = order.OrderNote,
                UserViewModel = new UserViewModel
                {
                    Id = order.CustomerId.ToString(),
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "User"
                },
                OrderItemViewModels = orderItems
            }
        };

        _mockOrderService
            .Setup(service => service.CreateOrderAsync(order, orderItems))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateOrder(createOrderModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<OrderResultViewModel>(okResult.Value);
        Assert.Equal(order.OrderDate, returnValue.OrderDate);
        Assert.Equal(OrderStatusEnum.Pending, returnValue.Status);
        Assert.Equal((PaymentMethod)order.PaymentMethod, returnValue.PaymentMethod);
        Assert.Equal(order.OrderNote, returnValue.OrderNote);
        Assert.NotNull(returnValue.UserViewModel);
        Assert.Equal(order.CustomerId.ToString(), returnValue.UserViewModel.Id);
        Assert.NotNull(returnValue.OrderItemViewModels);
        Assert.Single(returnValue.OrderItemViewModels);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var order = new OrderViewModel
        {
            CustomerId = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            PaymentMethod = PaymentMethodEnum.CreditCard,
            Status = OrderStatusEnum.Pending,
            OrderNote = "Test order",
            ShippingFee = 0
        };

        var orderItems = new List<OrderItemViewModel>();

        var createOrderModel = new CreateOrderViewModel
        {
            OrderViewModel = order,
            OrderItemViewModels = orderItems
        };

        var response = new ResponseMessage<OrderResultViewModel>
        {
            IsSuccess = false,
            Message = "Invalid order data",
            Data = null
        };

        _mockOrderService
            .Setup(service => service.CreateOrderAsync(order, orderItems))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateOrder(createOrderModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid order data", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateOrder_WithServiceException_ReturnsInternalServerError()
    {
        // Arrange
        var order = new OrderViewModel
        {
            CustomerId = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            PaymentMethod = PaymentMethodEnum.CreditCard,
            Status = OrderStatusEnum.Pending,
            OrderNote = "Test order",
            ShippingFee = 0
        };

        var orderItems = new List<OrderItemViewModel>
        {
            new()
            {
                BookId = Guid.NewGuid().ToString(),
                Title = "Test Book",
                AuthorName = "Test Author",
                CategoryName = "Test Category",
                Quantity = 1,
                UnitPrice = 100.00m
            }
        };

        var createOrderModel = new CreateOrderViewModel
        {
            OrderViewModel = order,
            OrderItemViewModels = orderItems
        };

        _mockOrderService
            .Setup(service => service.CreateOrderAsync(order, orderItems))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.CreateOrder(createOrderModel);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal("Service error", statusCodeResult.Value);
    }
} 