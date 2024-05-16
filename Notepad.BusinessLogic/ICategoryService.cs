using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    public interface ICategoryService
    {

        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDTO>> GetCategoriesByUserIdAsync(string userId);
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDto, string userId);
        Task UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task DeleteCategoryAsync(int id);
    }
}
