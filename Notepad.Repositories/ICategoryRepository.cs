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
        Task<Category> GetAsyncById(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<IEnumerable<Category>> GetCategoriesByUserId();
    }
}
