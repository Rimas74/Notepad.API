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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categories = await _categoryService.GetAllCategoriesAsync(userId);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }


        //[HttpGet("user/{userId}")]
        //public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesByUserIdAsync(string userId)
        //{
        //    var Id = HttpContext.User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;
        //    var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);
        //    return Ok(categories);
        //}

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<CategoryDTO>> CreateCategoryAsync([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDTO, userId);
            if (createdCategory == null)
            {
                return BadRequest("Failed to create category");
            }

            return Ok(createdCategory); // return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = createdCategory.CategoryId }, createdCategory);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDTO, userId);
            if (updatedCategory == null)
            {
                return BadRequest("Failed to update category");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);
            if (category == null)
            {
                return NotFound();
            }
            await _categoryService.DeleteCategoryAsync(id, userId);
            return NoContent();
        }
    }
}
