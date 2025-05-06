using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Models.enums;
using WebApplication1.Services.Services;
using OrderStatusEnum = SharedViewModal.ViewModels.OrderStatusEnum;


public class StatServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly StatService _statService;

        public StatServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _statService = new StatService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetStatsAsync_ShouldReturnCorrectStats()
        {
            // Arrange
            // Setup BookRepository mock
            var mockBookRepo = new Mock<IGenericRepository<Book>>();
            
            // Setup CountAsync for books
            mockBookRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
                .ReturnsAsync(10);

            // Create test book data
            var books = new List<Book>
            {
                new Book
                {
                    Id = "1",
                    Title = "Book 1",
                    CategoryId = 1,
                    CreatedDate = DateTime.Now.AddDays(-5),
                    UpdatedDate = DateTime.Now,
                    Quantity = 10,
                    ImageUrl = "image_1.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 1,
                        Name = "Category 1",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "2",
                    Title = "Book 2",
                    CategoryId = 1,
                    CreatedDate = DateTime.Now.AddDays(-4),
                    UpdatedDate = DateTime.Now,
                    Quantity = 10,
                    ImageUrl = "image_2.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 1,
                        Name = "Category 1",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "3",
                    Title = "Book 3",
                    CategoryId = 2,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    UpdatedDate = DateTime.Now,
                    Quantity = 10,
                    ImageUrl = "image_3.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 2,
                        Name = "Category 2",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "4",
                    Title = "Book 4",
                    CategoryId = 2,
                    CreatedDate = DateTime.Now.AddDays(-2),
                    UpdatedDate = DateTime.Now,
                    Quantity = 10,
                    ImageUrl = "image_4.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 2,
                        Name = "Category 2",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "5",
                    Title = "Book 5",
                    CategoryId = 3,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    UpdatedDate = DateTime.Now,
                    Quantity = 10,
                    ImageUrl = "image_5.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 3,
                        Name = "Category 3",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "6",
                    Title = "Book 6",
                    CategoryId = 3,
                    CreatedDate = DateTime.Now.AddDays(-6),
                    UpdatedDate = DateTime.Now.AddDays(-6),
                    Quantity = 10,
                    ImageUrl = "image_6.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 3,
                        Name = "Category 3",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "7",
                    Title = "Book 7",
                    CategoryId = 4,
                    CreatedDate = DateTime.Now.AddDays(-7),
                    UpdatedDate = DateTime.Now.AddDays(-7),
                    Quantity = 10,
                    ImageUrl = "image_7.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 4,
                        Name = "Category 4",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "8",
                    Title = "Book 8",
                    CategoryId = 4,
                    CreatedDate = DateTime.Now.AddDays(-8),
                    UpdatedDate = DateTime.Now.AddDays(-8),
                    Quantity = 10,
                    ImageUrl = "image_8.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 4,
                        Name = "Category 4",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "9",
                    Title = "Book 9",
                    CategoryId = 5,
                    CreatedDate = DateTime.Now.AddDays(-9),
                    UpdatedDate = DateTime.Now.AddDays(-9),
                    Quantity = 10,
                    ImageUrl = "image_9.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 5,
                        Name = "Category 5",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                },
                new Book
                {
                    Id = "10",
                    Title = "Book 10",
                    CategoryId = 5,
                    CreatedDate = DateTime.Now.AddDays(-10),
                    UpdatedDate = DateTime.Now.AddDays(-10),
                    Quantity = 10,
                    ImageUrl = "image_10.jpg",
                    IsDeleted = false,
                    Category = new Category
                    {
                        Id = 5,
                        Name = "Category 5",
                        IsDeleted = false
                    },
                    Author = new Author { IsDeleted = false }
                }
            };

            // Setup GetAllWithIncludeAsync for books
            mockBookRepo.Setup(repo => repo.GetAllWithIncludeAsync(
                    It.IsAny<Expression<Func<Book, bool>>>(),
                    It.IsAny<Expression<Func<Book, object>>>()))
                .ReturnsAsync(books);

            _mockUnitOfWork.Setup(uow => uow.BookRepository).Returns(mockBookRepo.Object);

            // Setup CategoryRepository mock
            var mockCategoryRepo = new Mock<IGenericRepository<Category>>();
            
            mockCategoryRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(5);

            _mockUnitOfWork.Setup(uow => uow.CategoryRepository).Returns(mockCategoryRepo.Object);

            // Setup UserRepository mock
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            
            mockUserRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(20);

            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);

            // Setup OrderRepository mock
            var mockOrderRepo = new Mock<IGenericRepository<Order>>();
            
            var orderItems = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 100m, Quantity = 5 },
                new OrderItem { UnitPrice = 200m, Quantity = 5 }
            };

            var orders = new List<Order>
            {
                new Order { Status = WebApplication1.Models.enums.OrderStatusEnum.Completed, OrderItems = orderItems }
            };

            mockOrderRepo.Setup(repo => repo.GetAllWithIncludeAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Expression<Func<Order, object>>>()))
                .ReturnsAsync(orders);

            _mockUnitOfWork.Setup(uow => uow.OrderRepository).Returns(mockOrderRepo.Object);

            // Act
            var result = await _statService.GetStatsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TotalBooks);
            Assert.Equal(5, result.TotalCategories);
            Assert.Equal(20, result.TotalCustomers);
            Assert.Equal(1500m, result.TotalRevenues);
            Assert.Equal(5, result.TopBooks.Count);
            Assert.Equal(5, result.TopCategories.Count);
        }

        [Fact]
        public async Task GetStatsAsync_WhenNoBooks_ShouldReturnEmptyLists()
        {
            // Arrange
            var mockBookRepo = new Mock<IGenericRepository<Book>>();
            var mockCategoryRepo = new Mock<IGenericRepository<Category>>();
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            var mockOrderRepo = new Mock<IGenericRepository<Order>>();

            mockBookRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
                .ReturnsAsync(0);
            mockCategoryRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(0);
            mockUserRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(0);
            
            mockBookRepo.Setup(repo => repo.GetAllWithIncludeAsync(
                    It.IsAny<Expression<Func<Book, bool>>>(),
                    It.IsAny<Expression<Func<Book, object>>>()))
                .ReturnsAsync(new List<Book>());
                
            mockOrderRepo.Setup(repo => repo.GetAllWithIncludeAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Expression<Func<Order, object>>>()))
                .ReturnsAsync(new List<Order>());

            _mockUnitOfWork.Setup(uow => uow.BookRepository).Returns(mockBookRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.CategoryRepository).Returns(mockCategoryRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.OrderRepository).Returns(mockOrderRepo.Object);

            // Act
            var result = await _statService.GetStatsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalBooks);
            Assert.Equal(0, result.TotalCategories);
            Assert.Equal(0, result.TotalCustomers);
            Assert.Equal(0, result.TotalRevenues);
            Assert.Empty(result.TopBooks);
            Assert.Empty(result.TopCategories);
        }
    }
