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

        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(string userId);
        Task<IEnumerable<CategoryDTO>> GetCategoriesByUserIdAsync(string userId);
        Task<CategoryDTO> GetCategoryByIdAsync(int id, string userId);
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDto, string userId);
        Task<CategoryDTO> UpdateCategoryAsync(int id, UpdateCategoryDTO updateCategoryDTO, string userId);
        Task DeleteCategoryAsync(int id, string userId);
    }
}
