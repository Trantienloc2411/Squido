using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
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
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Category>> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<IGenericRepository<Category>>();
            _mockMapper = new Mock<IMapper>();

            // Setup UnitOfWork to return the mocked CategoryRepository
            _mockUnitOfWork.Setup(u => u.CategoryRepository).Returns(_mockCategoryRepository.Object);

            // Initialize the service with mocked dependencies
            _categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsMappedCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new() { Id = 1, Name = "Category 1", IsDeleted = false },
                new() { Id = 2, Name = "Category 2", IsDeleted = false }
            };
            var categoryViewModels = new List<CategoryViewModel>
            {
                new() { Id = 1, Name = "Category 1" },
                new() { Id = 2, Name = "Category 2" }
            };

            _mockCategoryRepository
                .Setup(repo => repo.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<CategoryViewModel>>(categories))
                .Returns(categoryViewModels);

            // Act
            var result = await _categoryService.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Category 1", result.First().Name);
            Assert.Equal("Category 2", result.Last().Name);
            _mockCategoryRepository.Verify(repo => repo.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once());
            _mockMapper.Verify(m => m.Map<IEnumerable<CategoryViewModel>>(categories), Times.Once());
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>();
            var categoryViewModels = new List<CategoryViewModel>();

            _mockCategoryRepository
                .Setup(repo => repo.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<CategoryViewModel>>(categories))
                .Returns(categoryViewModels);

            // Act
            var result = await _categoryService.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCategoryRepository.Verify(repo => repo.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once());
            _mockMapper.Verify(m => m.Map<IEnumerable<CategoryViewModel>>(categories), Times.Once());
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            List<Category> categories = null;
            var categoryViewModels = new List<CategoryViewModel>();

            _mockCategoryRepository
                .Setup(repo => repo.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<CategoryViewModel>>(It.IsAny<object>()))
                .Returns(categoryViewModels);

            // Act
            var result = await _categoryService.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCategoryRepository.Verify(repo => repo.GetAllWithIncludeAsync(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once());
            _mockMapper.Verify(m => m.Map<IEnumerable<CategoryViewModel>>(It.IsAny<object>()), Times.Once());
        }
    }
}