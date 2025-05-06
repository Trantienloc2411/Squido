using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Services.Services;

namespace Squido.Test.UnitTests.Services;

public class BookServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Book>> _bookRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _bookRepositoryMock = new Mock<IGenericRepository<Book>>();
        _mapperMock = new Mock<IMapper>();

        // Setup IUnitOfWork.BookRepository to return the mocked IGenericRepository<Book>
        _unitOfWorkMock.Setup(u => u.BookRepository).Returns(_bookRepositoryMock.Object);

        _bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetBooks_NoKeyword_ReturnsAllNonDeletedBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book 
            { 
                Id = "1", 
                Title = "Book 1", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 1, 
                Description = "Book 1 desc", 
                Quantity = 10, 
                Price = 19.99m, 
                BuyCount = 5, 
                ImageUrl = "book1.jpg", 
                IsDeleted = false, 
                Author = new Author { Id = Guid.NewGuid(), FullName = "Author 1", IsDeleted = false }, 
                Category = new Category { Id = 1, Name = "Fiction", IsDeleted = false } 
            },
            new Book 
            { 
                Id = "2", 
                Title = "Book 2", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 2, 
                Description = "Book 2 desc", 
                Quantity = 15, 
                Price = 29.99m, 
                BuyCount = 3, 
                ImageUrl = "book2.jpg", 
                IsDeleted = false, 
                Author = new Author { Id = Guid.NewGuid(), FullName = "Author 2", IsDeleted = false }, 
                Category = new Category { Id = 2, Name = "Non-Fiction", IsDeleted = false } 
            }
        };
        var bookViewModels = books.Select(b => new BookViewModel 
        { 
            Id = b.Id, 
            Title = b.Title, 
            CategoryName = b.Category.Name, 
            AuthorId = b.AuthorId, 
            AuthorName = b.Author.FullName, 
            Quantity = b.Quantity, 
            Price = b.Price, 
            BuyCount = b.BuyCount, 
            ImageUrl = b.ImageUrl, 
            CreatedDate = b.CreatedDate 
        }).ToList();

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _mapperMock.Setup(m => m.Map<ICollection<BookViewModel>>(books)).Returns(bookViewModels);

        // Act
        var result = await _bookService.GetBooks(null);

        // Assert
        Assert.Equal(bookViewModels, result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBooks_WithKeyword_FiltersBooksByTitle()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book 
            { 
                Id = "1", 
                Title = "Book One", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 1, 
                Description = "Book 1 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { IsDeleted = false } 
            },
            new Book 
            { 
                Id = "2", 
                Title = "Another Book", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 2, 
                Description = "Book 2 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { IsDeleted = false } 
            }
        };
        var filteredBooks = books.Take(1).ToList();
        var bookViewModels = filteredBooks.Select(b => new BookViewModel 
        { 
            Id = b.Id, 
            Title = b.Title, 
            CategoryName = b.Category.Name, 
            AuthorId = b.AuthorId, 
            AuthorName = b.Author.FullName 
        }).ToList();

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _mapperMock.Setup(m => m.Map<ICollection<BookViewModel>>(It.IsAny<IEnumerable<Book>>())).Returns(bookViewModels);

        // Act
        var result = await _bookService.GetBooks("One");

        // Assert
        Assert.Single(result);
        Assert.Equal(bookViewModels, result);
    }

    [Fact]
    public async Task GetBooksPaginated_ReturnsPagedBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book 
            { 
                Id = "1", 
                Title = "Book 1", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 1, 
                Description = "Book 1 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { Id = 1, Name = "Fiction", IsDeleted = false } 
            },
            new Book 
            { 
                Id = "2", 
                Title = "Book 2", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 2, 
                Description = "Book 2 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { Id = 2, Name = "Non-Fiction", IsDeleted = false } 
            }
        };
        var bookViewModels = books.Select(b => new BookViewModel 
        { 
            Id = b.Id, 
            Title = b.Title, 
            CategoryName = b.Category.Name 
        }).ToList();

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _mapperMock.Setup(m => m.Map<ICollection<BookViewModel>>(books)).Returns(bookViewModels);

        // Act
        var result = await _bookService.GetBooksPaginated(1, 1, null);

        // Assert
        Assert.Single(result);
        Assert.Equal(bookViewModels[0], result.First());
    }

    [Fact]
    public async Task GetBookById_BookExists_ReturnsBook()
    {
        // Arrange
        var bookId = "1";
        var book = new Book 
        { 
            Id = bookId, 
            Title = "Book 1", 
            AuthorId = Guid.NewGuid(), 
            CategoryId = 1, 
            Description = "Book 1 desc", 
            Quantity = 10, 
            Price = 19.99m, 
            BuyCount = 5, 
            ImageUrl = "book1.jpg", 
            IsDeleted = false, 
            Author = new Author { IsDeleted = false }, 
            Category = new Category { IsDeleted = false } 
        };

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.GetBookById(bookId);

        // Assert
        Assert.Equal(book, result);
    }

    [Fact]
    public async Task GetBookById_ThrowsException_PropagatesException()
    {
        // Arrange
        var bookId = "1";
        var exception = new Exception("Database error");

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookService.GetBookById(bookId));
    }

    [Fact]
    public async Task GetBookByAuthorId_ValidAuthorId_ReturnsBooks()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var books = new List<Book>
        {
            new Book 
            { 
                Id = "1", 
                Title = "Book 1", 
                AuthorId = Guid.Parse(authorId), 
                CategoryId = 1, 
                Description = "Book 1 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { IsDeleted = false } 
            }
        };
        var bookViewModels = books.Select(b => new BookViewModel 
        { 
            Id = b.Id, 
            Title = b.Title, 
            CategoryName = b.Category.Name, 
            AuthorId = b.AuthorId, 
            AuthorName = b.Author.FullName 
        }).ToList();

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _mapperMock.Setup(m => m.Map<ICollection<BookViewModel>>(books)).Returns(bookViewModels);

        // Act
        var result = await _bookService.GetBookByAuthorId(authorId, null);

        // Assert
        Assert.Equal(bookViewModels, result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetBookByAuthorId_InvalidAuthorId_ThrowsArgumentException()
    {
        // Arrange
        var invalidAuthorId = "invalid-guid";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _bookService.GetBookByAuthorId(invalidAuthorId, null));
    }

    [Fact]
    public async Task GetBookByAuthorId_WithCurrentBook_ExcludesCurrentBook()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var currentBookId = "1";
        var books = new List<Book>
        {
            new Book 
            { 
                Id = "1", 
                Title = "Book 1", 
                AuthorId = Guid.Parse(authorId), 
                CategoryId = 1, 
                Description = "Book 1 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { IsDeleted = false } 
            },
            new Book 
            { 
                Id = "2", 
                Title = "Book 2", 
                AuthorId = Guid.Parse(authorId), 
                CategoryId = 2, 
                Description = "Book 2 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { IsDeleted = false } 
            }
        };
        var filteredBooks = books.Where(b => b.Id != currentBookId).ToList();
        var bookViewModels = filteredBooks.Select(b => new BookViewModel 
        { 
            Id = b.Id, 
            Title = b.Title, 
            CategoryName = b.Category.Name, 
            AuthorId = b.AuthorId, 
            AuthorName = b.Author.FullName 
        }).ToList();

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _mapperMock.Setup(m => m.Map<ICollection<BookViewModel>>(filteredBooks)).Returns(bookViewModels);

        // Act
        var result = await _bookService.GetBookByAuthorId(authorId, currentBookId);

        // Assert
        Assert.Single(result);
        Assert.Equal(bookViewModels, result);
    }

    [Fact]
    public async Task GetBooksByCategoryId_ValidCategoryIds_ReturnsBooks()
    {
        // Arrange
        var categoryIds = new List<int> { 1 };
        var books = new List<Book>
        {
            new Book 
            { 
                Id = "1", 
                Title = "Book 1", 
                AuthorId = Guid.NewGuid(), 
                CategoryId = 1, 
                Description = "Book 1 desc", 
                IsDeleted = false, 
                Author = new Author { IsDeleted = false }, 
                Category = new Category { Id = 1, Name = "Fiction", IsDeleted = false } 
            }
        };
        var bookViewModels = books.Select(b => new BookViewModel 
        { 
            Id = b.Id, 
            Title = b.Title, 
            CategoryName = b.Category.Name, 
            AuthorId = b.AuthorId, 
            AuthorName = b.Author.FullName 
        }).ToList();

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(books);
        _mapperMock.Setup(m => m.Map<ICollection<BookViewModel>>(books)).Returns(bookViewModels);

        // Act
        var result = await _bookService.GetBooksByCategoryId(categoryIds);

        // Assert
        Assert.Equal(bookViewModels, result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetBooksByCategoryId_ThrowsException_PropagatesException()
    {
        // Arrange
        var categoryIds = new List<int> { 1 };
        var exception = new Exception("Database error");

        _bookRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookService.GetBooksByCategoryId(categoryIds));
    }

    [Fact]
    public async Task CreateBook_ValidModel_ReturnsSuccessResponse()
    {
        // Arrange
        var bookViewModel = new CreateBookViewModel 
        { 
            Title = "New Book", 
            AuthorId = Guid.NewGuid(), 
            CategoryId = 1, 
            Description = "New book desc", 
            Quantity = 10, 
            Price = 19.99m, 
            ImageUrl = "newbook.jpg" 
        };
        var book = new Book 
        { 
            Id = "1", 
            Title = "New Book", 
            AuthorId = bookViewModel.AuthorId.Value, 
            CategoryId = bookViewModel.CategoryId.Value, 
            Description = "New book desc", 
            Quantity = 10, 
            Price = 19.99m, 
            ImageUrl = "newbook.jpg", 
            IsDeleted = false, 
            CreatedDate = DateTime.Now 
        };
        var bookViewModelResult = new BookViewModel 
        { 
            Id = book.Id, 
            Title = book.Title, 
            CategoryName = "Fiction", 
            AuthorId = book.AuthorId, 
            AuthorName = "Author 1", 
            Quantity = book.Quantity, 
            Price = book.Price, 
            ImageUrl = book.ImageUrl, 
            CreatedDate = book.CreatedDate 
        };

        _mapperMock.Setup(m => m.Map<Book>(bookViewModel)).Returns(book);
        _mapperMock.Setup(m => m.Map<BookViewModel>(book)).Returns(bookViewModelResult);
        _bookRepositoryMock.Setup(r => r.AddAsync(book)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

        // Act
        var result = await _bookService.CreateBook(bookViewModel);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Create Book Success", result.Message);
        Assert.Equal(bookViewModelResult, result.Data);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task CreateBook_ThrowsException_ReturnsFailure()
    {
        // Arrange
        var bookViewModel = new CreateBookViewModel { Title = "New Book", AuthorId = Guid.NewGuid(), CategoryId = 1 };
        var book = new Book { Id = "1", Title = "New Book", AuthorId = bookViewModel.AuthorId.Value, CategoryId = bookViewModel.CategoryId.Value };
        var exception = new Exception("Database error");

        _mapperMock.Setup(m => m.Map<Book>(bookViewModel)).Returns(book);
        _bookRepositoryMock.Setup(r => r.AddAsync(book)).ThrowsAsync(exception);

        // Act
        var result = await _bookService.CreateBook(bookViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Create Book Failed", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateBook_BookExists_ReturnsSuccessResponse()
    {
        // Arrange
        var bookId = "1";
        var bookViewModel = new CreateBookViewModel 
        { 
            Title = "Updated Book", 
            AuthorId = Guid.NewGuid(), 
            CategoryId = 1, 
            Description = "Updated desc", 
            Quantity = 20, 
            Price = 29.99m, 
            ImageUrl = "updated.jpg" 
        };
        var book = new Book 
        { 
            Id = bookId, 
            Title = "Old Book", 
            AuthorId = Guid.NewGuid(), 
            CategoryId = 1, 
            Description = "Old desc", 
            Quantity = 10, 
            Price = 19.99m, 
            ImageUrl = "old.jpg", 
            IsDeleted = false 
        };
        var updatedBookViewModel = new BookViewModel 
        { 
            Id = bookId, 
            Title = "Updated Book", 
            CategoryName = "Fiction", 
            AuthorId = bookViewModel.AuthorId.Value, 
            AuthorName = "Author 1", 
            Quantity = 20, 
            Price = 29.99m, 
            ImageUrl = "updated.jpg" 
        };

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(book);
        _mapperMock.Setup(m => m.Map(bookViewModel, book)).Returns(book);
        _mapperMock.Setup(m => m.Map<BookViewModel>(book)).Returns(updatedBookViewModel);
        _bookRepositoryMock.Setup(r => r.UpdateAsync(book)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

        // Act
        var result = await _bookService.UpdateBook(bookId, bookViewModel);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Update Book Success", result.Message);
        Assert.Equal(updatedBookViewModel, result.Data);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task UpdateBook_BookNotFound_ReturnsFailure()
    {
        // Arrange
        var bookId = "1";
        var bookViewModel = new CreateBookViewModel { Title = "Updated Book", AuthorId = Guid.NewGuid(), CategoryId = 1 };

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync((Book)null);

        // Act
        var result = await _bookService.UpdateBook(bookId, bookViewModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Book Not Found", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateBook_ThrowsException_PropagatesException()
    {
        // Arrange
        var bookId = "1";
        var bookViewModel = new CreateBookViewModel { Title = "Updated Book", AuthorId = Guid.NewGuid(), CategoryId = 1 };
        var exception = new Exception("Database error");

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookService.UpdateBook(bookId, bookViewModel));
    }

    [Fact]
    public async Task DeleteBook_BookExists_SetsIsDeletedAndReturnsSuccess()
    {
        // Arrange
        var bookId = "1";
        var book = new Book 
        { 
            Id = bookId, 
            Title = "Book 1", 
            AuthorId = Guid.NewGuid(), 
            CategoryId = 1, 
            Description = "Book 1 desc", 
            IsDeleted = false 
        };

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync(book);
        _bookRepositoryMock.Setup(r => r.UpdateAsync(book)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

        // Act
        var result = await _bookService.DeleteBook(bookId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(book.IsDeleted);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task DeleteBook_BookNotFound_ReturnsFailure()
    {
        // Arrange
        var bookId = "1";

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ReturnsAsync((Book)null);

        // Act
        var result = await _bookService.DeleteBook(bookId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Book Not Found", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteBook_ThrowsException_ReturnsFailure()
    {
        // Arrange
        var bookId = "1";
        var exception = new Exception("Database error");

        _bookRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<Expression<Func<Book, object>>[]>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _bookService.DeleteBook(bookId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exception.Message, result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public void CreateBookViewModel_ValidModel_PassesValidation()
    {
        // Arrange
        var model = new CreateBookViewModel 
        { 
            Title = "New Book", 
            AuthorId = Guid.NewGuid(), 
            CategoryId = 1, 
            Description = "New book desc", 
            Quantity = 10, 
            Price = 19.99m 
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}