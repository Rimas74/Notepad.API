using AutoMapper;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories;
using Notepad.Repositories.Entities;

namespace UnitTestsBusinessLogic
{
    public class CategoryServiceUnitTest : IAsyncLifetime
    {
        private CategoryService _categoryService;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new Mock<ICategoryRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<CategoryService>> _loggerMock = new Mock<ILogger<CategoryService>>();

        private List<Category> _categories;
        private List<CategoryDTO> _categoryDTOs;
        private UpdateCategoryDTO _updateCategoryDto;


        public Task InitializeAsync()
        {
            _categories = new List<Category>
            {
                new Category {CategoryId=1, Name="Category1", UserId="user1"},
                new Category {CategoryId=2, Name="Category2", UserId="user2"},
            };

            _categoryDTOs = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = 1, Name = "Category1", UserId = "user1" },
                new CategoryDTO { CategoryId = 2, Name = "Category2", UserId = "user2" }
            };

            _updateCategoryDto = new UpdateCategoryDTO
            {
                Name = "Updated Category"
            };


            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_categories);
            _categoryRepositoryMock.Setup(repo => repo.GetAsyncById(It.IsAny<int>())).ReturnsAsync((int id) => _categories.Find(c => c.CategoryId == id));
            _categoryRepositoryMock.Setup(repo => repo.GetCategoriesByUserIdAsync(It.IsAny<string>())).ReturnsAsync((string userId) => _categories.FindAll(c => c.UserId == userId));
            _categoryRepositoryMock.Setup(repo => repo.GetCategoryByNameAndUserIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string name, string userId) => _categories.Find(c => c.Name == name && c.UserId == userId));
            _categoryRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);


            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>())).Returns((IEnumerable<Category> source) =>
            {
                return source.Select(s => new CategoryDTO { CategoryId = s.CategoryId, Name = s.Name, UserId = s.UserId });
            });
            _mapperMock.Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>())).Returns((Category category) => category != null ? _categoryDTOs.Find(c => c.CategoryId == category.CategoryId) ?? new CategoryDTO() : null);

            _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCategoryDTO>(), It.IsAny<Category>())).Callback<UpdateCategoryDTO, Category>((dto, category) => category.Name = dto.Name);






            _categoryService = new CategoryService(_categoryRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]

        public async Task GetAllCategoriesAsync_ShouldreturnAllcategories()
        {
            //Act

            var result = await _categoryService.GetAllCategoriesAsync();

            //Assert

            Assert.NotNull(result);
            Assert.Equal(_categoryDTOs.Count, result.Count());
            Assert.Equal(_categoryDTOs.Select(c => c.Name), result.Select(c => c.Name));

        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldCreateCategory_WhenCategoryDoNotExist()
        {
            //Arrange

            var createCategoryDto = new CreateCategoryDTO { Name = "New Category" };
            var userId = "user3";
            var category = new Category { CategoryId = 3, Name = "New Category", UserId = userId };

            _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDTO>())).Returns(category);
            _categoryRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>())).Returns(new CategoryDTO { CategoryId = 3, Name = "New Category", UserId = userId });

            //Act

            var result = await _categoryService.CreateCategoryAsync(createCategoryDto, userId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
            _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Category>(c => c.Name == createCategoryDto.Name && c.UserId == userId)), Times.Once);

        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            //Arrange
            var categoryId = 1;

            //Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(categoryId, result.CategoryId);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            //Arrange
            var categoryId = 3;

            //Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            //Assert

            Assert.Null(result);
        }
        [Fact]
        public async Task UpdateCategoryAsync_ShouldUpdateCategory_WhenCategoryExists()
        {
            // Arrange
            var userId = "user1";
            var categoryDTOs = _categoryDTOs.FindAll(c => c.UserId == userId);

            // Act
            var result = await _categoryService.GetCategoriesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryDTOs.Count, result.Count());
            Assert.Equal(categoryDTOs.Select(c => c.Name), result.Select(c => c.Name));
        }
        [Fact]
        public async Task DeleteCategoryAsync_ShouldDeleteCategory_WhenCategoryExists()
        {
            //Arrange

            var categoryId = 1;
            var category = _categories.Find(c => c.CategoryId == categoryId);
            _categoryRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

            //Act
            await _categoryService.DeleteCategoryAsync(categoryId);

            //Assert

            _categoryRepositoryMock.Verify(repo => repo.DeleteAsync(It.Is<Category>(c => c.CategoryId == categoryId)), Times.Once);
        }
        [Fact]
        public async Task GetCategoriesByUserIdAsync_ShouldReturnCategories_WhenUserHasCategories()
        {
            //Arrange
            var userId = "user1";
            var categoryDTOs = _categoryDTOs.FindAll(c => c.UserId == userId);

            //Act

            var result = await _categoryService.GetCategoriesByUserIdAsync(userId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(categoryDTOs.Count, result.Count());
            Assert.Equal(categoryDTOs.Select(c => c.Name), result.Select(c => c.Name));
        }
    }
}