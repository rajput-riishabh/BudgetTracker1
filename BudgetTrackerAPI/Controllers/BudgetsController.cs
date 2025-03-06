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
    public class BudgetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Budgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets()
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId.Value)
                .Include(b => b.Category)
                .Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    CategoryName = b.Category.Name,
                    Amount = b.Amount,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                })
                .ToListAsync();

            return Ok(budgets);
        }

        // GET: api/Budgets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetDto>> GetBudgetById(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var budget = await _context.Budgets
                .Where(b => b.BudgetId == id && b.UserId == userId.Value)
                .Include(b => b.Category)
                .Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    CategoryName = b.Category.Name,
                    Amount = b.Amount,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                })
                .FirstOrDefaultAsync();

            if (budget == null)
            {
                return NotFound();
            }

            return Ok(budget);
        }

        // POST: api/Budgets (Create Budget - already implemented)
        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudget([FromBody] CreateBudgetRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var budget = new Budget()
            {
                UserId = userId.Value,
                CategoryId = model.CategoryId,
                Amount = model.Amount,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudgetById), new { id = budget.BudgetId }, budget);
        }

        // PUT: api/Budgets/{id} - Update Budget
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != model.BudgetId)
            {
                return BadRequest("BudgetId in request body does not match route parameter.");
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var existingBudget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId.Value);
            if (existingBudget == null)
            {
                return NotFound();
            }

            existingBudget.CategoryId = model.CategoryId;
            existingBudget.Amount = model.Amount;
            existingBudget.StartDate = model.StartDate;
            existingBudget.EndDate = model.EndDate;

            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/Budgets/{id} (Delete Budget - already implemented)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId.Value);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return NoContent();
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