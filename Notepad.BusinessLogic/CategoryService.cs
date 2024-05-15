using AutoMapper;
using Notepad.Common.DTOs;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    internal class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;
        public readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public Task CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            return _categoryRepository.AddAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetAsyncById(id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByUserIdAsync(string userId)
        {
            var categories = await _categoryRepository.GetCategoriesByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories); ;
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetAsyncById(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = await _categoryRepository.GetAsyncById(categoryDTO.CategoryId);
            _mapper.Map(categoryDTO, category);
            await _categoryRepository.UpdateAsync(category);
        }

    }
}
