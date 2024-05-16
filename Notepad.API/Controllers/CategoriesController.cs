using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Notepad.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        public readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllcategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesByUserIdAsync(string userId)
        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);
            return Ok(categories);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<CategoryDTO>> CreateCategoryAsync([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the authenticated user's ID

            var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDTO, userId);
            if (createdCategory == null)
            {
                return BadRequest("Failed to create category");
            }

            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = createdCategory.CategoryId }, createdCategory);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.CategoryId)
            {
                return BadRequest("No category with provided category id.");
            }
            await _categoryService.UpdateCategoryAsync(categoryDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
