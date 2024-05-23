using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetAsyncById(int id, string userId);
        Task<IEnumerable<Category>> GetAllAsync(string userId);
        Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<Category> GetCategoryByNameAndUserIdAsync(string name, string userId);

    }
}
