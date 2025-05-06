using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Services;
using SharedViewModal.ViewModels;
using Xunit;

namespace Squido.Test.UnitTests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepositoryMock = new Mock<IGenericRepository<Category>>();
            _mapperMock = new Mock<IMapper>();

            // Setup IUnitOfWork.CategoryRepository to return the mocked IGenericRepository<Category>
            _unitOfWorkMock.Setup(u => u.CategoryRepository).Returns(_categoryRepositoryMock.Object);

            _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsMappedCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = 1, Name = "Fiction", Description = "Fiction books", IsDeleted = false, Books = new List<Book>()
                },
                new Category
                {
                    Id = 2, Name = "Non-Fiction", Description = "Non-fiction books", IsDeleted = false,
                    Books = new List<Book>()
                }
            };
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                BookCount = c.Books.Count
            }).ToList();

            _categoryRepositoryMock.Setup(r => r.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync(categories);
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryViewModel>>(categories))
                .Returns(categoryViewModels);

            // Act
            var result = await _categoryService.GetCategories();

            // Assert
            Assert.Equal(categoryViewModels, result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateCategory_ValidModel_ReturnsSuccessResponse()
        {
            // Arrange
            var model = new CreateCategoryModel { Name = "Fiction", Description = "Fiction books" };
            var category = new Category
            {
                Id = 1,
                Name = "Fiction",
                Description = "Fiction books",
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Books = new List<Book>()
            };
            var mappedModel = new CreateCategoryModel { Name = "Fiction", Description = "Fiction books" };

            _mapperMock.Setup(m => m.Map<Category>(model)).Returns(category);
            _mapperMock.Setup(m => m.Map<CreateCategoryModel>(category)).Returns(mappedModel);
            _categoryRepositoryMock.Setup(r => r.AddAsync(category)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

            // Act
            var result = await _categoryService.CreateCategory(model);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mappedModel, result.Data);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
        }

        [Fact]
        public async Task CreateCategory_ThrowsException_PropagatesException()
        {
            // Arrange
            var model = new CreateCategoryModel { Name = "Fiction", Description = "Fiction books" };
            var category = new Category
                { Id = 1, Name = "Fiction", Description = "Fiction books", Books = new List<Book>() };
            var exception = new Exception("Database error");

            _mapperMock.Setup(m => m.Map<Category>(model)).Returns(category);
            _categoryRepositoryMock.Setup(r => r.AddAsync(category)).ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _categoryService.CreateCategory(model));
        }

        [Fact]
        public async Task UpdateCategory_CategoryExists_UpdatesAndReturnsSuccess()
        {
            // Arrange
            var categoryId = 1;
            var model = new CreateCategoryModel { Name = "Updated Fiction", Description = "Updated fiction books" };
            var existingCategory = new Category
            {
                Id = categoryId,
                Name = "Fiction",
                Description = "Fiction books",
                IsDeleted = false,
                CreatedDate = DateTime.Now.AddDays(-1),
                Books = new List<Book>()
            };
            var updatedCategory = new Category
            {
                Id = categoryId,
                Name = "Updated Fiction",
                Description = "Updated fiction books",
                IsDeleted = false,
                CreatedDate = existingCategory.CreatedDate,
                UpdatedDate = DateTime.Now,
                Books = new List<Book>()
            };
            var categoryViewModel = new CategoryViewModel
            {
                Id = categoryId,
                Name = "Updated Fiction",
                Description = "Updated fiction books",
                BookCount = 0
            };

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync(existingCategory);
            _mapperMock.Setup(m => m.Map<Category>(model)).Returns(updatedCategory);
            _mapperMock.Setup(m => m.Map<CategoryViewModel>(updatedCategory)).Returns(categoryViewModel);
            _categoryRepositoryMock.Setup(r => r.UpdateAsync(updatedCategory)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

            // Act
            var result = await _categoryService.UpdateCategory(categoryId, model);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(categoryViewModel, result.Data);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
        }

        [Fact]
        public async Task UpdateCategory_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            var categoryId = 1;
            var model = new CreateCategoryModel { Name = "Fiction", Description = "Fiction books" };

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _categoryService.UpdateCategory(categoryId, model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateCategory_ThrowsException_ReturnsFailure()
        {
            // Arrange
            var categoryId = 1;
            var model = new CreateCategoryModel { Name = "Fiction", Description = "Fiction books" };
            var exception = new Exception("Database error");

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _categoryService.UpdateCategory(categoryId, model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exception.Message, result.ExceptionMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetCategoryById_CategoryExists_ReturnsSuccessWithData()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category
            {
                Id = categoryId,
                Name = "Fiction",
                Description = "Fiction books",
                IsDeleted = false,
                Books = new List<Book>()
            };
            var categoryViewModel = new CategoryViewModel
            {
                Id = categoryId,
                Name = "Fiction",
                Description = "Fiction books",
                BookCount = 0
            };

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryViewModel>(category)).Returns(categoryViewModel);

            // Act
            var result = await _categoryService.GetCategoryById(categoryId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(categoryViewModel, result.Data);
        }

        [Fact]
        public async Task GetCategoryById_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            var categoryId = 1;

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _categoryService.GetCategoryById(categoryId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetCategoryById_ThrowsException_PropagatesException()
        {
            // Arrange
            var categoryId = 1;
            var exception = new Exception("Database error");

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _categoryService.GetCategoryById(categoryId));
        }

        [Fact]
        public async Task DeleteCategory_CategoryExists_SetsIsDeletedAndReturnsSuccess()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category
            {
                Id = categoryId,
                Name = "Fiction",
                Description = "Fiction books",
                IsDeleted = false,
                Books = new List<Book>()
            };

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync(category);
            _categoryRepositoryMock.Setup(r => r.UpdateAsync(category)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save()).Verifiable();

            // Act
            var result = await _categoryService.DeleteCategory(categoryId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(category.IsDeleted);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
        }

        [Fact]
        public async Task DeleteCategory_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            var categoryId = 1;

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _categoryService.DeleteCategory(categoryId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task DeleteCategory_ThrowsException_ReturnsFailure()
        {
            // Arrange
            var categoryId = 1;
            var exception = new Exception("Database error");

            _categoryRepositoryMock.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Expression<Func<Category, object>>[]>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _categoryService.DeleteCategory(categoryId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exception.Message, result.ExceptionMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public void CreateCategoryModel_ValidModel_PassesValidation()
        {
            // Arrange
            var model = new CreateCategoryModel { Name = "Fiction", Description = "Fiction books" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void CreateCategoryModel_InvalidName_FailsValidation()
        {
            // Arrange
            var model = new CreateCategoryModel { Name = "Fi", Description = "Fiction books" }; // Name too short
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Name"));
        }

        [Fact]
        public void CreateCategoryModel_NullDescription_FailsValidation()
        {
            // Arrange
            var model = new CreateCategoryModel { Name = "Fiction", Description = null };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Description"));
        }
    }
}