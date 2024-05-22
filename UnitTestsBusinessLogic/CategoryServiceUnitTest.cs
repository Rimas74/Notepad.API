using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsBusinessLogic
{
    public class CategoryServiceUnitTest : IAsyncLifetime
    {
        private NotepadContext _context;
        private CategoryService _categoryService;

        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<CategoryService>> _loggerMock = new Mock<ILogger<CategoryService>>();

        private List<CategoryDTO> _categoryDTOs;
        private UpdateCategoryDTO _updateCategoryDTO;
        private CreateCategoryDTO _createCategoryDTO;

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();

            _categoryDTOs = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = 1, Name = "Category1", UserId = "user1" },
                new CategoryDTO { CategoryId = 2, Name = "Category2", UserId = "user2" }
            };

            _updateCategoryDTO = new UpdateCategoryDTO
            {
                Name = "Updated Category"
            };

            _createCategoryDTO = new CreateCategoryDTO
            {
                Name = "New Category"
            };

            var categoryRepositoryMock = new CategoryRepository(_context);

            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>())).Returns((IEnumerable<Category> source) =>
            {
                return source.Select(s => new CategoryDTO { CategoryId = s.CategoryId, Name = s.Name, UserId = s.UserId });
            });

            _mapperMock.Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>())).Returns((Category category) =>
                category != null ? new CategoryDTO { CategoryId = category.CategoryId, Name = category.Name, UserId = category.UserId } : null);

            _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDTO>())).Returns((CreateCategoryDTO dto) =>
                new Category { Name = dto.Name, UserId = "user3" });

            _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCategoryDTO>(), It.IsAny<Category>())).Callback<UpdateCategoryDTO, Category>((dto, category) =>
            {
                category.Name = dto.Name;
            });

            _categoryService = new CategoryService(categoryRepositoryMock, _mapperMock.Object, _loggerMock.Object);
        }

        private async Task<NotepadContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<NotepadContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new NotepadContext(options);
            context.Database.EnsureCreated();

            if (!context.Categories.Any())
            {
                context.Categories.AddRange
                (
                    new Category { CategoryId = 1, Name = "Category1", UserId = "user1" },
                    new Category { CategoryId = 2, Name = "Category2", UserId = "user2" }
                );
                await context.SaveChangesAsync();
            }
            return context;
        }

        public Task DisposeAsync()
        {
            _context?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
        {
            //Act
            var result = await _categoryService.GetAllCategoriesAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(_categoryDTOs.Count, result.Count());
            Assert.Equal(_categoryDTOs.Select(c => c.Name), result.Select(c => c.Name));
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
        public async Task CreateCategoryAsync_ShouldCreateCategory_WhenCategoryDoesNotExist()
        {
            //Arrange
            var userId = "user3";

            //Act
            var result = await _categoryService.CreateCategoryAsync(_createCategoryDTO, userId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(_createCategoryDTO.Name, result.Name);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldUpdateCategory_WhenCategoryExists()
        {
            //Arrange
            var categoryId = 1;

            //Act
            await _categoryService.UpdateCategoryAsync(categoryId, _updateCategoryDTO, "user1");

            //Assert
            var updatedCategory = await _context.Categories.FindAsync(categoryId);
            Assert.Equal(_updateCategoryDTO.Name, updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldDeleteCategory_WhenCategoryExists()
        {
            //Arrange
            var categoryId = 1;

            //Act
            await _categoryService.DeleteCategoryAsync(categoryId);

            //Assert
            var deletedCategory = await _context.Categories.FindAsync(categoryId);
            Assert.Null(deletedCategory);
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
