using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Notepad.API.Controllers;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsAPI
{
    public class CategoriesControllerUnitTest : IAsyncLifetime
    {
        private NotepadContext _context;
        private CategoriesController _categoriesController;

        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<CategoryService>> _loggerMock = new Mock<ILogger<CategoryService>>();
        private List<CategoryDTO> _categoryDTOs;
        private List<Category> _categories;
        private CreateCategoryDTO _createCategoryDTO;
        private UpdateCategoryDTO _updateCategoryDTO;

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();

            _categoryDTOs = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = 1, Name = "Category1", UserId = "user1" },
                new CategoryDTO { CategoryId = 2, Name = "Category2", UserId = "user2" }
            };

            _categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category1", UserId = "user1" },
                new Category { CategoryId = 2, Name = "Category2", UserId = "user2" }
            };

            _createCategoryDTO = new CreateCategoryDTO
            {
                Name = "New Category"
            };

            _updateCategoryDTO = new UpdateCategoryDTO
            {
                Name = "Updated Category"
            };

            var categoryRepositoryMock = new CategoryRepository(_context);

            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>())).Returns((IEnumerable<Category> source) =>
            {
                return source.Select(s => new CategoryDTO { CategoryId = s.CategoryId, Name = s.Name, UserId = s.UserId }).ToList();
            });

            _mapperMock.Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>())).Returns((Category category) =>
                category != null ? new CategoryDTO { CategoryId = category.CategoryId, Name = category.Name, UserId = category.UserId } : null);

            _mapperMock.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDTO>())).Returns((CreateCategoryDTO dto) =>
                new Category { Name = dto.Name, UserId = "user1" });

            _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCategoryDTO>(), It.IsAny<Category>())).Callback<UpdateCategoryDTO, Category>((dto, category) =>
            {
                category.Name = dto.Name;
            });

            var categoryService = new CategoryService(categoryRepositoryMock, _mapperMock.Object, _loggerMock.Object);

            _categoriesController = new CategoriesController(categoryService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "user1")
                        }, "mock"))
                    }
                }
            };
        }

        private async Task<NotepadContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<NotepadContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
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
        public async Task GetAllCategoriesAsync_ShouldReturnOk_WhenCategoriesExist()
        {
            // Act
            var result = await _categoriesController.GetAllcategoriesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<CategoryDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnOk_WhenCategoryExists()
        {
            // Act
            var result = await _categoriesController.GetCategoryByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);
            Assert.Equal(1, returnValue.CategoryId);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Act
            var result = await _categoriesController.GetCategoryByIdAsync(3);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCategoriesByUserIdAsync_ShouldReturnOk_WhenUserHasCategories()
        {
            // Act
            var result = await _categoriesController.GetCategoriesByUserIdAsync("user1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<CategoryDTO>>(okResult.Value);
            Assert.Equal(1, returnValue.Count);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnOk_WhenCategoryIsCreated()
        {
            // Act
            var result = await _categoriesController.CreateCategoryAsync(_createCategoryDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);
            Assert.Equal(_createCategoryDTO.Name, returnValue.Name);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNoContent_WhenCategoryIsUpdated()
        {
            // Act
            var result = await _categoriesController.UpdateCategory(1, _updateCategoryDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnNoContent_WhenCategoryIsDeleted()
        {
            // Act
            var result = await _categoriesController.DeleteCategoryAsync(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Act
            var result = await _categoriesController.DeleteCategoryAsync(3);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
