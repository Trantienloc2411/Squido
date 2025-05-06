using System.Linq.Expressions;
using AutoMapper;
using Moq;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Services;
using OrderStatusEnum = WebApplication1.Models.enums.OrderStatusEnum;
using PaymentMethodEnum = WebApplication1.Models.enums.PaymentMethodEnum;

namespace WebApplication1.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IGenericRepository<OrderItem>> _orderItemRepositoryMock;
        private readonly Mock<IGenericRepository<Book>> _bookRepositoryMock;
        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepositoryMock = new Mock<IGenericRepository<Order>>();
            _orderItemRepositoryMock = new Mock<IGenericRepository<OrderItem>>();
            _bookRepositoryMock = new Mock<IGenericRepository<Book>>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.OrderRepository).Returns(_orderRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.OrderItemRepository).Returns(_orderItemRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.BookRepository).Returns(_bookRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);

            _orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderViewModel = new OrderViewModel
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.CreditCard,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                OrderNote = "Test order"
            };
            var orderItemViewModels = new List<OrderItemViewModel>
            {
                new OrderItemViewModel { BookId = "1", Quantity = 2 },
                new OrderItemViewModel { BookId = "2", Quantity = 1 }
            };
            var order = new Order
            {
                Id = "12345678920250506",
                CustomerId = customerId,
                OrderDate = (DateTime)orderViewModel.OrderDate,
                PaymentMethod = PaymentMethodEnum.CreditCard,
                Status = OrderStatusEnum.Pending,
                OrderNote = orderViewModel.OrderNote,
                OrderItems = new List<OrderItem>()
            };
            var books = new List<Book>
            {
                new Book
                {
                    Id = "1", Title = "Book 1", Price = 20.00m,
                    Author = new Author { FullName = "Author 1", IsDeleted = false },
                    Category = new Category { Name = "Fiction", IsDeleted = false }, IsDeleted = false
                },
                new Book
                {
                    Id = "2", Title = "Book 2", Price = 15.00m,
                    Author = new Author { FullName = "Author 2", IsDeleted = false },
                    Category = new Category { Name = "Fiction", IsDeleted = false }, IsDeleted = false
                }
            };
            var user = new User { Id = customerId, Email = "user@example.com", Orders = new List<Order>() };
            var orderResultViewModel = new OrderResultViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                PaymentMethod = PaymentMethod.CreditCard,
                UserViewModel = new UserViewModel { Id = user.Id.ToString(), Email = user.Email },
                OrderNote = order.OrderNote,
                OrderItemViewModels = new List<OrderItemViewModel>
                {
                    new OrderItemViewModel
                    {
                        BookId = "1", Title = "Book 1", AuthorName = "Author 1", CategoryName = "Fiction", Quantity = 2,
                        UnitPrice = 20.00m
                    },
                    new OrderItemViewModel
                    {
                        BookId = "2", Title = "Book 2", AuthorName = "Author 2", CategoryName = "Fiction", Quantity = 1,
                        UnitPrice = 15.00m
                    }
                }
            };

            _mapperMock.Setup(m => m.Map<Order>(orderViewModel)).Returns(order);
            _mapperMock.Setup(m => m.Map<UserViewModel>(user)).Returns(orderResultViewModel.UserViewModel);
            _mapperMock.Setup(m => m.Map<List<OrderItemViewModel>>(It.IsAny<List<OrderItem>>()))
                .Returns(orderResultViewModel.OrderItemViewModels);
            _mapperMock.Setup(m => m.Map<OrderResultViewModel>(order)).Returns(orderResultViewModel);
            _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.Is<Expression<Func<Book, bool>>>(expr => expr.Compile()(books[0])),
                It.IsAny<Expression<Func<Book, object>>[]>())).ReturnsAsync(books);
            _userRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(
                It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(user)),
                It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _orderItemRepositoryMock.Setup(r => r.AddAsync(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save());

            // Act
            var result = await _orderService.CreateOrderAsync(orderViewModel, orderItemViewModels);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Order created successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(orderResultViewModel.Id, result.Data.Id);
            Assert.Equal(orderResultViewModel.OrderDate, result.Data.OrderDate);
            Assert.Equal(orderResultViewModel.Status, result.Data.Status);
            Assert.Equal(orderResultViewModel.PaymentMethod, result.Data.PaymentMethod);
            Assert.Equal(orderResultViewModel.OrderNote, result.Data.OrderNote);
            Assert.Equal(orderResultViewModel.OrderItemViewModels.Count, result.Data.OrderItemViewModels.Count);
            _orderRepositoryMock.Verify(r => r.AddAsync(It.Is<Order>(o => o.Id == order.Id)), Times.Once());
            _orderItemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OrderItem>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.Save(), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.Dispose(), Times.Once());
        }

        [Fact]
        public async Task CreateOrderAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderViewModel = new OrderViewModel
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.CreditCard,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending
            };
            var orderItemViewModels = new List<OrderItemViewModel>
            {
                new OrderItemViewModel { BookId = "1", Quantity = 1 }
            };
            var order = new Order
            {
                Id = "12345678920250506",
                CustomerId = customerId,
                OrderDate = (DateTime)orderViewModel.OrderDate,
                PaymentMethod = PaymentMethodEnum.CreditCard,
                Status = OrderStatusEnum.Pending
            };
            var books = new List<Book>
            {
                new Book
                {
                    Id = "1", Title = "Book 1", Price = 20.00m, Author = new Author { IsDeleted = false },
                    Category = new Category { IsDeleted = false }, IsDeleted = false
                }
            };

            _mapperMock.Setup(m => m.Map<Order>(orderViewModel)).Returns(order);
            _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>[]>())).ReturnsAsync(books);
            _userRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User)null);
            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _orderItemRepositoryMock.Setup(r => r.AddAsync(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save());

            // Act
            var result = await _orderService.CreateOrderAsync(orderViewModel, orderItemViewModels);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.Message);
            Assert.Null(result.Data);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.Dispose(), Times.Once());
        }

        [Fact]
        public async Task CreateOrderAsync_InvalidBookId_ReturnsSuccessWithEmptyItems()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderViewModel = new OrderViewModel
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.CreditCard,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending
            };
            var orderItemViewModels = new List<OrderItemViewModel>
            {
                new OrderItemViewModel { BookId = "999", Quantity = 1 }
            };
            var order = new Order
            {
                Id = "12345678920250506",
                CustomerId = customerId,
                OrderDate = (DateTime)orderViewModel.OrderDate,
                PaymentMethod = PaymentMethodEnum.CreditCard,
                Status = OrderStatusEnum.Pending,
                OrderItems = new List<OrderItem>()
            };
            var books = new List<Book>();
            var user = new User { Id = customerId, Email = "user@example.com", Orders = new List<Order>() };
            var orderResultViewModel = new OrderResultViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                PaymentMethod = PaymentMethod.CreditCard,
                UserViewModel = new UserViewModel { Id = user.Id.ToString(), Email = user.Email },
                OrderItemViewModels = new List<OrderItemViewModel>()
            };

            _mapperMock.Setup(m => m.Map<Order>(orderViewModel)).Returns(order);
            _mapperMock.Setup(m => m.Map<UserViewModel>(user)).Returns(orderResultViewModel.UserViewModel);
            _mapperMock.Setup(m => m.Map<List<OrderItemViewModel>>(It.IsAny<List<OrderItem>>()))
                .Returns(orderResultViewModel.OrderItemViewModels);
            _mapperMock.Setup(m => m.Map<OrderResultViewModel>(order)).Returns(orderResultViewModel);
            _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>[]>())).ReturnsAsync(books);
            _userRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save());

            // Act
            var result = await _orderService.CreateOrderAsync(orderViewModel, orderItemViewModels);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Order created successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.OrderItemViewModels);
            _orderItemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OrderItem>()), Times.Never());
            _unitOfWorkMock.Verify(u => u.Save(), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.Dispose(), Times.Once());
        }

        [Fact]
        public async Task CreateOrderAsync_Exception_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderViewModel = new OrderViewModel
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.CreditCard,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending
            };
            var orderItemViewModels = new List<OrderItemViewModel>
            {
                new OrderItemViewModel { BookId = "1", Quantity = 1 }
            };
            var exception = new Exception("Database error");

            _mapperMock.Setup(m => m.Map<Order>(orderViewModel)).Throws(exception);

            // Act
            var result = await _orderService.CreateOrderAsync(orderViewModel, orderItemViewModels);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Message);
            Assert.Null(result.Data);
            _unitOfWorkMock.Verify(u => u.Dispose(), Times.Once());
        }

        [Fact]
        public async Task GetOrderAsync_OrderExists_ReturnsSuccess()
        {
            // Arrange
            var orderId = "12345678920250506";
            var order = new Order
            {
                Id = orderId,
                CustomerId = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatusEnum.Pending,
                PaymentMethod = PaymentMethodEnum.CreditCard,
                OrderItems = new List<OrderItem>()
            };
            var orderResultViewModel = new OrderResultViewModel
            {
                Id = orderId,
                OrderDate = order.OrderDate,
                Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                PaymentMethod = PaymentMethod.CreditCard,
                OrderItemViewModels = new List<OrderItemViewModel>()
            };

            _orderRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(
                It.Is<Expression<Func<Order, bool>>>(expr => expr.Compile()(order)),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync(order);
            _mapperMock.Setup(m => m.Map<OrderResultViewModel>(order)).Returns(orderResultViewModel);

            // Act
            var result = await _orderService.GetOrderAsync(orderId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Order found", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(orderId, result.Data.Id);
            Assert.Equal(orderResultViewModel.Status, result.Data.Status);
        }

        [Fact]
        public async Task GetOrderAsync_OrderNotFound_ReturnsFailure()
        {
            // Arrange
            var orderId = "nonexistent";

            _orderRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.GetOrderAsync(orderId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Order not found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderAsync_Exception_ReturnsFailure()
        {
            // Arrange
            var orderId = "12345678920250506";
            var exception = new Exception("Database error");

            _orderRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ThrowsAsync(exception);

            // Act
            var result = await _orderService.GetOrderAsync(orderId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderByUserIdAsync_OrdersExist_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "1", CustomerId = userId, OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Pending,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.CreditCard
                },
                new Order
                {
                    Id = "2", CustomerId = userId, OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Completed,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.Paypal
                }
            };
            var orderResultViewModels = new List<OrderResultViewModel>
            {
                new OrderResultViewModel
                {
                    Id = "1", Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                    OrderDate = orders[0].OrderDate, PaymentMethod = PaymentMethod.CreditCard,
                    OrderItemViewModels = new List<OrderItemViewModel>()
                },
                new OrderResultViewModel
                {
                    Id = "2", Status = SharedViewModal.ViewModels.OrderStatusEnum.Completed,
                    OrderDate = orders[1].OrderDate, PaymentMethod = PaymentMethod.Paypal,
                    OrderItemViewModels = new List<OrderItemViewModel>()
                }
            };

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.Is<Expression<Func<Order, bool>>>(expr => expr.Compile()(orders[0])),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<List<OrderResultViewModel>>(orders)).Returns(orderResultViewModels);

            // Act
            var result = await _orderService.GetOrderByUserIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Order found", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("1", result.Data[0].Id);
            Assert.Equal("2", result.Data[1].Id);
        }

        [Fact]
        public async Task GetOrderByUserIdAsync_NoOrders_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order>();

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetOrderByUserIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Order not found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderByUserIdAsync_Exception_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var exception = new Exception("Database error");

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ThrowsAsync(exception);

            // Act
            var result = await _orderService.GetOrderByUserIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetAllOrdersAsync_NoKeyword_ReturnsAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "1", OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Pending,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.CreditCard
                },
                new Order
                {
                    Id = "2", OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Completed,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.Paypal
                }
            };
            var orderViewModels = new List<OrderViewModel>
            {
                new OrderViewModel
                {
                    Id = "1", Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                    OrderDate = orders[0].OrderDate,
                    PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.CreditCard
                },
                new OrderViewModel
                {
                    Id = "2", Status = SharedViewModal.ViewModels.OrderStatusEnum.Completed,
                    OrderDate = orders[1].OrderDate, PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.Paypal
                }
            };

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<List<OrderViewModel>>(orders)).Returns(orderViewModels);

            // Act
            var result = await _orderService.GetAllOrdersAsync(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("1", result.Data[0].Id);
            Assert.Equal("2", result.Data[1].Id);
        }

        [Fact]
        public async Task GetAllOrdersAsync_WithKeyword_ReturnsFilteredOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "123abc", OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Pending,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.CreditCard
                },
                new Order
                {
                    Id = "456def", OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Completed,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.Paypal
                }
            };
            var filteredOrders = new List<Order> { orders[0] };
            var orderViewModels = new List<OrderViewModel>
            {
                new OrderViewModel
                {
                    Id = "123abc", Status = SharedViewModal.ViewModels.OrderStatusEnum.Pending,
                    OrderDate = orders[0].OrderDate, PaymentMethod = SharedViewModal.ViewModels.PaymentMethodEnum.CreditCard
                }
            };

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync(orders);
            _mapperMock.Setup(m =>
                    m.Map<List<OrderViewModel>>(It.Is<IEnumerable<Order>>(o => o.SequenceEqual(filteredOrders))))
                .Returns(orderViewModels);

            // Act
            var result = await _orderService.GetAllOrdersAsync("123");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("123abc", result.Data[0].Id);
        }

        [Fact]
        public async Task GetAllOrdersAsync_NoMatchingOrders_ReturnsEmptySuccess()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "456def", OrderDate = DateTime.UtcNow, Status = OrderStatusEnum.Completed,
                    OrderItems = new List<OrderItem>(), PaymentMethod = PaymentMethodEnum.Paypal
                }
            };
            var orderViewModels = new List<OrderViewModel>();

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<List<OrderViewModel>>(It.Is<IEnumerable<Order>>(o => !o.Any())))
                .Returns(orderViewModels);

            // Act
            var result = await _orderService.GetAllOrdersAsync("123");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Exception_ReturnsFailure()
        {
            // Arrange
            var exception = new Exception("Database error");

            _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Expression<Func<Order, object>>[]>())).ThrowsAsync(exception);

            // Act
            var result = await _orderService.GetAllOrdersAsync(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Message);
            Assert.Null(result.Data);
        }
    }
}
