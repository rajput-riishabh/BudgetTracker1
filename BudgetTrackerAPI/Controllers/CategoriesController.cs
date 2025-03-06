using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Models;
using BudgetTrackerAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var categories = await _context.Categories
                .Where(c => c.IsPredefined || c.UserId == userId.Value)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    IsPredefined = c.IsPredefined
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var category = await _context.Categories
                .Where(c => c.CategoryId == id && (c.IsPredefined || c.UserId == userId.Value)) // Ensure category is predefined OR belongs to user
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    IsPredefined = c.IsPredefined
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }


        // POST: api/Categories (Create Category - already implemented)
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            if (await _context.Categories.AnyAsync(c => c.UserId == userId.Value && c.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Category name already exists for this user.");
                return Conflict(ModelState);
            }

            var category = new Category()
            {
                UserId = userId.Value,
                Name = model.Name,
                IsPredefined = false
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                IsPredefined = category.IsPredefined
            };

            return CreatedAtAction(nameof(GetCategories), null, categoryDto);
        }


        // PUT: api/Categories/{id} - Update Category
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequestDto model) // Re-use CreateCategoryRequestDto for update
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id <= 4) // Prevent updating predefined categories (IDs 1-4 are predefined)
            {
                return BadRequest("Predefined categories cannot be updated.");
            }


            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId.Value); // Ensure user owns category
            if (existingCategory == null)
            {
                return NotFound(); // Category not found or not user's category
            }

            // Check for duplicate name update within user's categories
            if (await _context.Categories.AnyAsync(c => c.UserId == userId.Value && c.Name == model.Name && c.CategoryId != id)) // Exclude current category ID in check
            {
                ModelState.AddModelError("Name", "Category name already exists for this user.");
                return Conflict(ModelState); // 409 Conflict
            }


            existingCategory.Name = model.Name; // Update name
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - successful update
        }


        // DELETE: api/Categories/{id} - Delete Category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 4) // Prevent deleting predefined categories (IDs 1-4 are predefined)
            {
                return BadRequest("Predefined categories cannot be deleted.");
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId.Value); // Verify user owns category

            if (category == null)
            {
                return NotFound(); // Category not found or not user's category
            }

            // Check if category is used in any expenses or budgets before deleting
            bool isCategoryInUse = await _context.Expenses.AnyAsync(e => e.CategoryId == id) ||
                                     await _context.Budgets.AnyAsync(b => b.CategoryId == id);

            if (isCategoryInUse)
            {
                return BadRequest("Cannot delete category as it is used in expenses or budgets."); // Prevent delete if in use
            }


            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - successful delete
        }


        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}