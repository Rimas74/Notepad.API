using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;
        public readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDto, string userId)
        {
            _logger.LogInformation($"Creating category for userId={userId}");
            var existingCategory = await _categoryRepository.GetCategoryByNameAndUserIdAsync(createCategoryDto.Name, userId);
            if (existingCategory != null)
            {
                _logger.LogWarning($"Category with name {createCategoryDto.Name} already exists for userId={userId}");
                return null;
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            category.UserId = userId;
            await _categoryRepository.AddAsync(category);
            _logger.LogInformation($"Category with id={category.CategoryId} created for userId={userId}");
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task DeleteCategoryAsync(int id, string userId)
        {
            _logger.LogInformation($"Deleting category with id={id}");
            var category = await _categoryRepository.GetAsyncById(id, userId);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
                _logger.LogInformation($"Category with id={id} deleted");
            }
            else
            {
                _logger.LogWarning($"Category with id={id} not found");
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(string userId)
        {
            _logger.LogInformation("Getting all categories");
            var categories = await _categoryRepository.GetAllAsync(userId);

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByUserIdAsync(string userId)
        {
            _logger.LogInformation($"Getting categories for userId={userId}");
            var categories = await _categoryRepository.GetCategoriesByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories); ;
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id, string userId)
        {

            _logger.LogInformation($"Getting category by id={id}");
            var category = await _categoryRepository.GetAsyncById(id, userId);
            if (category == null)
            {
                _logger.LogWarning($"Category with id={id} not found");
            }
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int id, UpdateCategoryDTO updateCategoryDto, string userId)
        {
            _logger.LogInformation($"Updating category with id={id} for userId={userId}");
            var category = await _categoryRepository.GetAsyncById(id, userId);
            if (category == null)
            {
                _logger.LogWarning($"Category with id={id} not found");
                return null;
            }
            if (category.UserId != userId)
            {
                _logger.LogWarning($"User with userId={userId} does not have permission to update category with id={id}");
                return null;
            }

            category.Name = updateCategoryDto.Name;
            await _categoryRepository.UpdateAsync(category);
            _logger.LogInformation($"Category with id={id} updated");
            return _mapper.Map<CategoryDTO>(category);

        }

    }
}
