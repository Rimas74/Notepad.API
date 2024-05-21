using Microsoft.EntityFrameworkCore;
using Notepad.DataAccess;
using Notepad.Repositories.Entities;

namespace UnitTestsDataAccsess
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
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {

            var categories = await _repository.GetAllAsync();

            Assert.Equal(2, categories.Count());
        }

        [Fact]
        public async Task GetAsyncById_ShouldReturnCategoryById()
        {

            var category = await _repository.GetAsyncById(1);

            Assert.NotNull(category);

            Assert.Equal(1, category.CategoryId);
        }

        [Fact]
        public async Task GetCategoriesByUserIdAsync_ShouldReturnCategoriesByUserId()
        {
            var categories = await _repository.GetCategoriesByUserIdAsync("user1");

            Assert.Single(categories);
        }

        [Fact]
        public async Task GetCategoryByNameAndUserIdAsync_ShouldReturnCategoriesByNameUserId()
        {

            var category = await _repository.GetCategoryByNameAndUserIdAsync("Category1", "user1");
            Assert.NotNull(category);
            Assert.Equal("Category1", category.Name);
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