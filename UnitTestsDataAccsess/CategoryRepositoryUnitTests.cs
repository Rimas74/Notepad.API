using Microsoft.EntityFrameworkCore;
using Notepad.DataAccess;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsDataAccess
{
    public class CategoryRepositoryUnitTests : IAsyncLifetime
    {
        private NotepadContext _context;
        private CategoryRepository _repository;

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();
            _repository = new CategoryRepository(_context);
        }

        public Task DisposeAsync()
        {
            _context?.Dispose();
            return Task.CompletedTask;
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
                context.Categories.AddRange(
                    new Category { CategoryId = 1, Name = "Category1", UserId = "user1" },
                    new Category { CategoryId = 2, Name = "Category2", UserId = "user2" }
                );
                await context.SaveChangesAsync();
            }
            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddCategory()
        {
            var category = new Category { Name = "NewCategory", UserId = "user3" };

            await _repository.AddAsync(category);
            var categories = await _context.Categories.ToListAsync();

            Assert.Contains(category, categories);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCategory()
        {
            var category = await _context.Categories.FirstAsync();

            await _repository.DeleteAsync(category);
            var categories = await _context.Categories.ToListAsync();

            Assert.DoesNotContain(category, categories);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategoriesForUser()
        {
            var userId = "user1";
            var categories = await _repository.GetAllAsync(userId);

            Assert.Single(categories);
            Assert.All(categories, c => Assert.Equal(userId, c.UserId));
        }

        [Fact]
        public async Task GetAsyncById_ShouldReturnCategoryByIdAndUserId()
        {
            var userId = "user1";
            var category = await _repository.GetAsyncById(1, userId);

            Assert.NotNull(category);
            Assert.Equal(1, category.CategoryId);
            Assert.Equal(userId, category.UserId);
        }

        [Fact]
        public async Task GetCategoriesByUserIdAsync_ShouldReturnCategoriesByUserId()
        {
            var userId = "user1";
            var categories = await _repository.GetCategoriesByUserIdAsync(userId);

            Assert.Single(categories);
            Assert.All(categories, c => Assert.Equal(userId, c.UserId));
        }

        [Fact]
        public async Task GetCategoryByNameAndUserIdAsync_ShouldReturnCategoriesByNameAndUserId()
        {
            var userId = "user1";
            var category = await _repository.GetCategoryByNameAndUserIdAsync("Category1", userId);

            Assert.NotNull(category);
            Assert.Equal("Category1", category.Name);
            Assert.Equal(userId, category.UserId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCategory()
        {
            var category = await _context.Categories.FirstAsync();
            category.Name = "UpdatedCategory";

            await _repository.UpdateAsync(category);
            var updatedCategory = await _context.Categories.FindAsync(category.CategoryId);

            Assert.Equal("UpdatedCategory", updatedCategory.Name);
        }
    }
}
