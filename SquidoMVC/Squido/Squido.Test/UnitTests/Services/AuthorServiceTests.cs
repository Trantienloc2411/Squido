using System.Linq.Expressions;
using AutoMapper;
using Moq;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Services.Services;

namespace Squido.Test.UnitTests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Author>> _authorRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authorRepositoryMock = new Mock<IGenericRepository<Author>>();
        _mapperMock = new Mock<IMapper>();

        // Setup IUnitOfWork.AuthorRepository to return the mocked IGenericRepository<Author>
        _unitOfWorkMock.Setup(u => u.AuthorRepository).Returns(_authorRepositoryMock.Object);

        _authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAuthor_ValidAuthor_ReturnsSuccessResponse()
    {
        // Arrange
        var author = new Author 
        { 
            Id = Guid.NewGuid(), 
            FullName = "John Doe", 
            Bio = "Test bio", 
            Books = new List<Book>() 
        };
        var authorViewModel = new AuthorViewModel 
        { 
            Id = author.Id.ToString(), 
            FullName = author.FullName, 
            Bio = author.Bio, 
            Books = new List<BookViewModel>() 
        };
        
        _mapperMock.Setup(m => m.Map<AuthorViewModel>(author)).Returns(authorViewModel);
        _authorRepositoryMock.Setup(r => r.AddAsync(author)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

        // Act
        var result = await _authorService.CreateAuthor(author);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(authorViewModel, result.Data);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task CreateAuthor_ThrowsException_PropagatesException()
    {
        // Arrange
        var author = new Author { Id = Guid.NewGuid(), FullName = "John Doe" };
        var exception = new Exception("Database error");
        
        _authorRepositoryMock.Setup(r => r.AddAsync(author)).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authorService.CreateAuthor(author));
    }

    [Fact]
    public async Task DeleteAuthor_AuthorExists_SetsIsDeletedAndReturnsSuccess()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var author = new Author 
        { 
            Id = Guid.Parse(authorId), 
            FullName = "John Doe", 
            IsDeleted = false 
        };
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync(author);
        _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

        // Act
        var result = await _authorService.DeleteAuthor(authorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(author.IsDeleted);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task DeleteAuthor_AuthorNotFound_ReturnsSuccess()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync((Author)null);

        // Act
        var result = await _authorService.DeleteAuthor(authorId);

        // Assert
        Assert.True(result.IsSuccess);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task DeleteAuthor_ThrowsException_ReturnsFailure()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var exception = new Exception("Database error");
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _authorService.DeleteAuthor(authorId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exception.Message, result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetAuthorById_AuthorExists_ReturnsSuccessWithData()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var author = new Author 
        { 
            Id = Guid.Parse(authorId), 
            FullName = "John Doe", 
            Bio = "Test bio", 
            Books = new List<Book>() 
        };
        var authorViewModel = new AuthorViewModel 
        { 
            Id = authorId, 
            FullName = author.FullName, 
            Bio = author.Bio, 
            Books = new List<BookViewModel>() 
        };
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync(author);
        _mapperMock.Setup(m => m.Map<AuthorViewModel>(author)).Returns(authorViewModel);

        // Act
        var result = await _authorService.GetAuthorById(authorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(authorViewModel, result.Data);
    }

    [Fact]
    public async Task GetAuthorById_AuthorNotFound_ReturnsFailure()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync((Author)null);

        // Act
        var result = await _authorService.GetAuthorById(authorId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetAuthorById_ThrowsException_PropagatesException()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var exception = new Exception("Database error");
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authorService.GetAuthorById(authorId));
    }

    [Fact]
    public async Task GetAuthors_NoKeyword_ReturnsPagedAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new Author { Id = Guid.NewGuid(), FullName = "John Doe", IsDeleted = false, Books = new List<Book>() },
            new Author { Id = Guid.NewGuid(), FullName = "Jane Doe", IsDeleted = false, Books = new List<Book>() }
        };
        var authorViewModels = authors.Select(a => new AuthorViewModel 
        { 
            Id = a.Id.ToString(), 
            FullName = a.FullName, 
            Books = new List<BookViewModel>() 
        }).ToList();
        
        _authorRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync(authors);
        _mapperMock.Setup(m => m.Map<ICollection<AuthorViewModel>>(It.IsAny<IEnumerable<Author>>()))
            .Returns(authorViewModels);

        // Act
        var result = await _authorService.GetAuthors(null, 1, 10);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(authorViewModels, result);
    }

    [Fact]
    public async Task GetAuthors_WithKeyword_FiltersAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new Author { Id = Guid.NewGuid(), FullName = "John Doe", IsDeleted = false, Books = new List<Book>() },
            new Author { Id = Guid.NewGuid(), FullName = "Jane Smith", IsDeleted = false, Books = new List<Book>() }
        };
        var filteredAuthor = authors[0];
        var filteredViewModel = new List<AuthorViewModel> 
        { 
            new AuthorViewModel 
            { 
                Id = filteredAuthor.Id.ToString(), 
                FullName = filteredAuthor.FullName, 
                Books = new List<BookViewModel>() 
            } 
        };
        
        _authorRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync(authors);
        _mapperMock.Setup(m => m.Map<ICollection<AuthorViewModel>>(It.IsAny<IEnumerable<Author>>()))
            .Returns(filteredViewModel);

        // Act
        var result = await _authorService.GetAuthors("Doe", 1, 10);

        // Assert
        Assert.Single(result);
        Assert.Equal(filteredViewModel, result);
    }

    [Fact]
    public async Task UpdateAuthor_AuthorExists_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var existingAuthor = new Author 
        { 
            Id = Guid.Parse(authorId), 
            FullName = "John Doe", 
            Bio = "Old bio", 
            Books = new List<Book>() 
        };
        var updatedAuthor = new Author 
        { 
            FullName = "John Updated", 
            Bio = "New bio" 
        };
        var updatedViewModel = new AuthorViewModel 
        { 
            Id = authorId, 
            FullName = "John Updated", 
            Bio = "New bio", 
            Books = new List<BookViewModel>() 
        };
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync(existingAuthor);
        _mapperMock.Setup(m => m.Map(updatedAuthor, existingAuthor)).Returns(existingAuthor);
        _mapperMock.Setup(m => m.Map<AuthorViewModel>(existingAuthor)).Returns(updatedViewModel);
        _authorRepositoryMock.Setup(r => r.UpdateAsync(existingAuthor)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

        // Act
        var result = await _authorService.UpdateAuthor(authorId, updatedAuthor);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updatedViewModel, result.Data);
        Assert.Equal("Author updated successfully", result.Message);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public async Task UpdateAuthor_AuthorNotFound_ReturnsFailure()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var updatedAuthor = new Author { FullName = "John Updated" };
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ReturnsAsync((Author)null);

        // Act
        var result = await _authorService.UpdateAuthor(authorId, updatedAuthor);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Author not found", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAuthor_ThrowsException_ReturnsFailure()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var updatedAuthor = new Author { FullName = "John Updated" };
        var exception = new Exception("Database error");
        
        _authorRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<Expression<Func<Author, object>>[]>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _authorService.UpdateAuthor(authorId, updatedAuthor);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exception.Message, result.Message);
        Assert.Null(result.Data);
    }
}

// Provided model classes




