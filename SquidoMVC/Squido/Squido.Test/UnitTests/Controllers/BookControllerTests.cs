using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Services.Interfaces;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using Xunit;

namespace Squido.Test.UnitTests.Controllers;

public class BookControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BookController _controller;

    public BookControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new BookController(_mockBookService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetBooks_WithPagination_ReturnsPaginatedResult()
    {
        // Arrange
        var keyword = "test";
        var page = 1;
        var pageSize = 10;
        var books = new List<BookViewModel>
        {
            new() { Id = "1", Title = "Book 1" },
            new() { Id = "2", Title = "Book 2" }
        };

        _mockBookService
            .Setup(service => service.GetBooks(keyword))
            .ReturnsAsync(books);

        _mockBookService
            .Setup(service => service.GetBooksPaginated(page, pageSize, keyword))
            .ReturnsAsync(books);

        // Act
        var result = await _controller.GetBooks(keyword, page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PaginationViewModel<BookViewModel>>(okResult.Value);
        Assert.Equal(2, returnValue.Data.Count);
        Assert.Equal(1, returnValue.CurrentPage);
        Assert.Equal(1, returnValue.PageCount);
        Assert.Equal(2, returnValue.TotalCount);
    }

    [Fact]
    public async Task GetBooks_WithoutPagination_ReturnsAllBooks()
    {
        // Arrange
        var keyword = "test";
        var books = new List<BookViewModel>
        {
            new() { Id = "1", Title = "Book 1" },
            new() { Id = "2", Title = "Book 2" }
        };

        _mockBookService
            .Setup(service => service.GetBooks(keyword))
            .ReturnsAsync(books);

        // Act
        var result = await _controller.GetBooks(keyword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetBook_WithValidId_ReturnsBookDetails()
    {
        // Arrange
        var bookId = "1";
        var authorId = Guid.NewGuid();
        var book = new Book 
        { 
            Id = bookId, 
            Title = "Test Book",
            AuthorId = authorId,
            Author = new Author { Id = authorId, Bio = "Test Bio" },
            Category = new Category(),
            Description = "Test Description"
        };
        var bookViewModel = new BookViewModel 
        { 
            Id = bookId, 
            Title = "Test Book"
        };
        var relatedBooks = new List<BookViewModel>();
        var categoryViewModel = new CategoryViewModel();
        var viewBookDetailViewModel = new ViewBookDetailViewModel
        {
            Book = bookViewModel,
            Category = categoryViewModel,
            Bio = "Test Bio",
            RatingValueAverage = 4.5,
            BookDescription = "Test Description",
            BookRelated = relatedBooks
        };

        _mockBookService
            .Setup(service => service.GetBookById(bookId))
            .ReturnsAsync(book);

        _mockBookService
            .Setup(service => service.GetBookByAuthorId(authorId.ToString(), bookId))
            .ReturnsAsync(relatedBooks);

        _mockMapper
            .Setup(m => m.Map<BookViewModel>(It.IsAny<Book>()))
            .Returns(bookViewModel);

        _mockMapper
            .Setup(m => m.Map<CategoryViewModel>(It.IsAny<Category>()))
            .Returns(categoryViewModel);

        _mockMapper
            .Setup(m => m.Map<ViewBookDetailViewModel>(It.IsAny<Book>()))
            .Returns(new ViewBookDetailViewModel
            {
                Book = bookViewModel,
                Category = categoryViewModel,
                Bio = "Test Bio",
                RatingValueAverage = 4.5,
                BookDescription = "Test Description",
                BookRelated = relatedBooks
            });

        // Act
        var result = await _controller.GetBook(bookId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ViewBookDetailViewModel>(okResult.Value);
        Assert.NotNull(returnValue.Book);
        Assert.Equal(bookId, returnValue.Book.Id);
        Assert.Equal("Test Book", returnValue.Book.Title);
        Assert.Equal(4.5, returnValue.RatingValueAverage);
        Assert.Equal("Test Bio", returnValue.Bio);
        Assert.Equal("Test Description", returnValue.BookDescription);
        Assert.Empty(returnValue.BookRelated);
    }

    [Fact]
    public async Task GetBook_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var bookId = "invalid";
        _mockBookService
            .Setup(service => service.GetBookById(bookId))
            .ReturnsAsync((Book)null);

        // Act
        var result = await _controller.GetBook(bookId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
} 