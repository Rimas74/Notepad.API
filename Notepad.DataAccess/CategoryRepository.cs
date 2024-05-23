using Microsoft.EntityFrameworkCore;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.DataAccess
{
    public class CategoryRepository : ICategoryRepository
    {
        public readonly NotepadContext _context;

        public CategoryRepository(NotepadContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync(string userId)
        {
            return await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<Category> GetAsyncById(int id, string userId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId)
        {
            return await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<Category> GetCategoryByNameAndUserIdAsync(string name, string userId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name && c.UserId == userId);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
