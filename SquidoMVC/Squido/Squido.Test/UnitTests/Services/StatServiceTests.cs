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
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Book>> _bookRepositoryMock;
    private readonly Mock<IGenericRepository<Category>> _categoryRepositoryMock;
    private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
    private readonly Mock<IGenericRepository<Order>> _orderRepositoryMock;
    private readonly StatService _statService;

    public StatServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _bookRepositoryMock = new Mock<IGenericRepository<Book>>();
        _categoryRepositoryMock = new Mock<IGenericRepository<Category>>();
        _userRepositoryMock = new Mock<IGenericRepository<User>>();
        _orderRepositoryMock = new Mock<IGenericRepository<Order>>();

        _unitOfWorkMock.Setup(u => u.BookRepository).Returns(_bookRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.CategoryRepository).Returns(_categoryRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.OrderRepository).Returns(_orderRepositoryMock.Object);

        _statService = new StatService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsCorrectStats()
    {
        // Arrange
        var author1Id = Guid.NewGuid();
        var author2Id = Guid.NewGuid();
        var author3Id = Guid.NewGuid();
        var books = new List<Book>
        {
            new Book { Id = "1", Title = "Book 1", CategoryId = 1, IsDeleted = false, CreatedDate = DateTime.UtcNow.AddDays(-10), UpdatedDate = DateTime.UtcNow.AddDays(-2), Quantity = 100, Price = 20.00m, BuyCount = 50, ImageUrl = "img1.jpg", AuthorId = author1Id, Category = new Category { Id = 1, Name = "Fiction", IsDeleted = false }, Author = new Author { Id = author1Id, FullName = "Author 1", IsDeleted = false } },
            new Book { Id = "2", Title = "Book 2", CategoryId = 1, IsDeleted = false, CreatedDate = DateTime.UtcNow.AddDays(-8), UpdatedDate = DateTime.UtcNow.AddDays(-1), Quantity = 50, Price = 15.00m, BuyCount = 30, ImageUrl = "img2.jpg", AuthorId = author2Id, Category = new Category { Id = 1, Name = "Fiction", IsDeleted = false }, Author = new Author { Id = author2Id, FullName = "Author 2", IsDeleted = false } },
            new Book { Id = "3", Title = "Book 3", CategoryId = 2, IsDeleted = false, CreatedDate = DateTime.UtcNow.AddDays(-5), UpdatedDate = DateTime.UtcNow.AddDays(-3), Quantity = 30, Price = 25.00m, BuyCount = 20, ImageUrl = "img3.jpg", AuthorId = author3Id, Category = new Category { Id = 2, Name = "Non-Fiction", IsDeleted = false }, Author = new Author { Id = author3Id, FullName = "Author 3", IsDeleted = false } },
            new Book { Id = "4", Title = "Book 4", CategoryId = 1, IsDeleted = true, CreatedDate = DateTime.UtcNow.AddDays(-12), UpdatedDate = DateTime.UtcNow.AddDays(-4), Quantity = 20, Price = 10.00m, BuyCount = 10, ImageUrl = "img4.jpg", AuthorId = Guid.NewGuid(), Category = new Category { Id = 1, Name = "Fiction", IsDeleted = false }, Author = new Author { Id = Guid.NewGuid(), FullName = "Author 4", IsDeleted = false } }
        };
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Fiction", IsDeleted = false },
            new Category { Id = 2, Name = "Non-Fiction", IsDeleted = false },
            new Category { Id = 3, Name = "Deleted Category", IsDeleted = true }
        };
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), RoleId = 1, IsDeleted = false, Email = "user1@example.com", Gender = GenderEnum.Male, Password = "pass", Role = new Role { Id = 1, RoleName = "Customer" } },
            new User { Id = Guid.NewGuid(), RoleId = 1, IsDeleted = false, Email = "user2@example.com", Gender = GenderEnum.Female, Password = "pass", Role = new Role { Id = 1, RoleName = "Customer" } },
            new User { Id = Guid.NewGuid(), RoleId = 2, IsDeleted = false, Email = "admin@example.com", Gender = GenderEnum.Male, Password = "pass", Role = new Role { Id = 2, RoleName = "Admin" } }
        };
        var orders = new List<Order>
        {
            new Order
            {
                Id = "O1",
                CustomerId = users[0].Id,
                Status = WebApplication1.Models.enums.OrderStatusEnum.Completed,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Id = Guid.NewGuid(), OrderId = "O1", BookId = "1", UnitPrice = 20.00m, Quantity = 2, Title = "Book 1", AuthorName = "Author 1", CategoryName = "Fiction" },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = "O1", BookId = "2", UnitPrice = 15.00m, Quantity = 1, Title = "Book 2", AuthorName = "Author 2", CategoryName = "Fiction" }
                }
            },
            new Order
            {
                Id = "O2",
                CustomerId = users[1].Id,
                Status = WebApplication1.Models.enums.OrderStatusEnum.Completed,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Id = Guid.NewGuid(), OrderId = "O2", BookId = "3", UnitPrice = 25.00m, Quantity = 1, Title = "Book 3", AuthorName = "Author 3", CategoryName = "Non-Fiction" }
                }
            }
        };

        var expectedStatViewModel = new StatViewModel
        {
            TotalBooks = 3,
            TotalCategories = 2,
            TotalCustomers = 2,
            TotalRevenues = (20.00m * 2) + (15.00m * 1) + (25.00m * 1), // 40 + 15 + 25 = 80
            TopBooks = new List<BookViewModel>
            {
                new BookViewModel { Id = "2", Title = "Book 2", CategoryName = "Fiction", AuthorId = Guid.Empty, AuthorName = null, Quantity = 50, Price = 0, BuyCount = 0, ImageUrl = "img2.jpg", CreatedDate = books[1].CreatedDate, UpdatedDate = books[1].UpdatedDate },
                new BookViewModel { Id = "1", Title = "Book 1", CategoryName = "Fiction", AuthorId = Guid.Empty, AuthorName = null, Quantity = 100, Price = 0, BuyCount = 0, ImageUrl = "img1.jpg", CreatedDate = books[0].CreatedDate, UpdatedDate = books[0].UpdatedDate },
                new BookViewModel { Id = "3", Title = "Book 3", CategoryName = "Non-Fiction", AuthorId = Guid.Empty, AuthorName = null, Quantity = 30, Price = 0, BuyCount = 0, ImageUrl = "img3.jpg", CreatedDate = books[2].CreatedDate, UpdatedDate = books[2].UpdatedDate }
            },
            TopCategories = new List<CategoryViewModel>
            {
                new CategoryViewModel { Id = 1, Name = "Fiction", Description = null, BookCount = 2 },
                new CategoryViewModel { Id = 2, Name = "Non-Fiction", Description = null, BookCount = 1 }
            }
        };

        // Setup predicates
        Expression<Func<Book, bool>> bookCountPredicate = b => !b.IsDeleted && !b.Category.IsDeleted && !b.Author.IsDeleted;
        Expression<Func<Book, bool>> bookAllPredicate = b => !b.IsDeleted;
        Expression<Func<Category, bool>> categoryPredicate = c => !c.IsDeleted;
        Expression<Func<User, bool>> userPredicate = c => !c.IsDeleted && c.RoleId == 1;
        Expression<Func<Order, bool>> orderPredicate = o => o.Status == WebApplication1.Models.enums.OrderStatusEnum.Completed;

        _bookRepositoryMock.Setup(r => r.CountAsync(bookCountPredicate)).ReturnsAsync(3);
        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(bookAllPredicate, It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books.Where(b => !b.IsDeleted).ToList());
        _categoryRepositoryMock.Setup(r => r.CountAsync(categoryPredicate)).ReturnsAsync(2);
        _userRepositoryMock.Setup(r => r.CountAsync(userPredicate)).ReturnsAsync(2);
        _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(orderPredicate, It.IsAny<Expression<Func<Order, object>>[]>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _statService.GetStatsAsync();

        // Assert
        Assert.Equal(expectedStatViewModel.TotalBooks, result.TotalBooks);
        Assert.Equal(expectedStatViewModel.TotalCategories, result.TotalCategories);
        Assert.Equal(expectedStatViewModel.TotalCustomers, result.TotalCustomers);
        Assert.Equal(expectedStatViewModel.TotalRevenues, result.TotalRevenues);
        Assert.Equal(expectedStatViewModel.TopBooks.Count, result.TopBooks.Count);
        for (int i = 0; i < expectedStatViewModel.TopBooks.Count; i++)
        {
            Assert.Equal(expectedStatViewModel.TopBooks[i].Id, result.TopBooks[i].Id);
            Assert.Equal(expectedStatViewModel.TopBooks[i].Title, result.TopBooks[i].Title);
            Assert.Equal(expectedStatViewModel.TopBooks[i].CategoryName, result.TopBooks[i].CategoryName);
            Assert.Equal(expectedStatViewModel.TopBooks[i].AuthorId, result.TopBooks[i].AuthorId);
            Assert.Equal(expectedStatViewModel.TopBooks[i].AuthorName, result.TopBooks[i].AuthorName);
            Assert.Equal(expectedStatViewModel.TopBooks[i].Quantity, result.TopBooks[i].Quantity);
            Assert.Equal(expectedStatViewModel.TopBooks[i].Price, result.TopBooks[i].Price);
            Assert.Equal(expectedStatViewModel.TopBooks[i].BuyCount, result.TopBooks[i].BuyCount);
            Assert.Equal(expectedStatViewModel.TopBooks[i].ImageUrl, result.TopBooks[i].ImageUrl);
            Assert.Equal(expectedStatViewModel.TopBooks[i].CreatedDate, result.TopBooks[i].CreatedDate);
            Assert.Equal(expectedStatViewModel.TopBooks[i].UpdatedDate, result.TopBooks[i].UpdatedDate);
        }
        Assert.Equal(expectedStatViewModel.TopCategories.Count, result.TopCategories.Count);
        for (int i = 0; i < expectedStatViewModel.TopCategories.Count; i++)
        {
            Assert.Equal(expectedStatViewModel.TopCategories[i].Id, result.TopCategories[i].Id);
            Assert.Equal(expectedStatViewModel.TopCategories[i].Name, result.TopCategories[i].Name);
            Assert.Equal(expectedStatViewModel.TopCategories[i].Description, result.TopCategories[i].Description);
            Assert.Equal(expectedStatViewModel.TopCategories[i].BookCount, result.TopCategories[i].BookCount);
        }
        _unitOfWorkMock.Verify(u => u.Dispose(), Times.Never());
    }

    [Fact]
    public async Task GetStatsAsync_NoData_ReturnsZerosAndEmptyCollections()
    {
        // Arrange
        var books = new List<Book>();
        var categories = new List<Category>();
        var users = new List<User>();
        var orders = new List<Order>();

        var expectedStatViewModel = new StatViewModel
        {
            TotalBooks = 0,
            TotalCategories = 0,
            TotalCustomers = 0,
            TotalRevenues = 0,
            TopBooks = new List<BookViewModel>(),
            TopCategories = new List<CategoryViewModel>()
        };

        Expression<Func<Book, bool>> bookCountPredicate = b => !b.IsDeleted && !b.Category.IsDeleted && !b.Author.IsDeleted;
        Expression<Func<Book, bool>> bookAllPredicate = b => !b.IsDeleted;
        Expression<Func<Category, bool>> categoryPredicate = c => !c.IsDeleted;
        Expression<Func<User, bool>> userPredicate = c => !c.IsDeleted && c.RoleId == 1;
        Expression<Func<Order, bool>> orderPredicate = o => o.Status == WebApplication1.Models.enums.OrderStatusEnum.Completed;

        _bookRepositoryMock.Setup(r => r.CountAsync(bookCountPredicate)).ReturnsAsync(0);
        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(bookAllPredicate, It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _categoryRepositoryMock.Setup(r => r.CountAsync(categoryPredicate)).ReturnsAsync(0);
        _userRepositoryMock.Setup(r => r.CountAsync(userPredicate)).ReturnsAsync(0);
        _orderRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(orderPredicate, It.IsAny<Expression<Func<Order, object>>[]>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _statService.GetStatsAsync();

        // Assert
        Assert.Equal(expectedStatViewModel.TotalBooks, result.TotalBooks);
        Assert.Equal(expectedStatViewModel.TotalCategories, result.TotalCategories);
        Assert.Equal(expectedStatViewModel.TotalCustomers, result.TotalCustomers);
        Assert.Equal(expectedStatViewModel.TotalRevenues, result.TotalRevenues);
        Assert.Empty(result.TopBooks);
        Assert.Empty(result.TopCategories);
        _unitOfWorkMock.Verify(u => u.Dispose(), Times.Never());
    }

    [Fact]
    public async Task GetStatsAsync_ThrowsException_ReturnsDefaultStatViewModel()
    {
        // Arrange
        var exception = new Exception("Database error");
        Expression<Func<Book, bool>> bookCountPredicate = b => !b.IsDeleted && !b.Category.IsDeleted && !b.Author.IsDeleted;

        _bookRepositoryMock.Setup(r => r.CountAsync(bookCountPredicate)).ThrowsAsync(exception);

        // Act
        var result = await _statService.GetStatsAsync();

        // Assert
        Assert.Equal(0, result.TotalBooks);
        Assert.Equal(0, result.TotalCategories);
        Assert.Equal(0, result.TotalCustomers);
        Assert.Equal(0, result.TotalRevenues);
        Assert.Empty(result.TopBooks);
        Assert.Empty(result.TopCategories);
        _unitOfWorkMock.Verify(u => u.Dispose(), Times.Never());
    }
}

// Provided StatService implementation
public class StatService
{
    private readonly IUnitOfWork unitOfWork;

    public StatService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<StatViewModel> GetStatsAsync()
    {
        var totalBooks = await unitOfWork.BookRepository.CountAsync(b => b.IsDeleted == false && b.Category.IsDeleted == false && b.Author.IsDeleted == false);
        var totalCategories = await unitOfWork.CategoryRepository.CountAsync(t => t.IsDeleted == false);
        var totalCustomers = await unitOfWork.UserRepository.CountAsync(c => c.IsDeleted == false && c.RoleId == 1);

        var getAllOrders = await unitOfWork.OrderRepository.GetAllWithIncludeAsync(o => o.Status == WebApplication1.Models.enums.OrderStatusEnum.Completed, o => o.OrderItems);

        var totalRevenues = getAllOrders.Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity));

        var allBook = await unitOfWork.BookRepository.GetAllWithIncludeAsync(b => b.IsDeleted == false, b => b.Category);

        var topBooks = allBook
            .OrderByDescending(b => b.UpdatedDate)
            .Take(5)
            .Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                CategoryName = b.Category.Name,
                ImageUrl = b.ImageUrl,
                Quantity = b.Quantity,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate
            });
        var topCategories = allBook
            .GroupBy(b => b.CategoryId)
            .Select(g => new CategoryViewModel
            {
                Id = g.Key,
                Name = g.FirstOrDefault()!.Category.Name,
                BookCount = g.Count()
            })
            .OrderByDescending(c => c.BookCount)
            .Take(5);

        return new StatViewModel
        {
            TotalBooks = totalBooks,
            TotalCategories = totalCategories,
            TotalCustomers = totalCustomers,
            TotalRevenues = totalRevenues,
            TopBooks = [.. topBooks],
            TopCategories = [.. topCategories]
        };
    }


}
