using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Models;
using BudgetTrackerAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace BudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses(
            [FromQuery] DateTime? startDate,      // Optional start date filter
            [FromQuery] DateTime? endDate,        // Optional end date filter
            [FromQuery] int? categoryId         // Optional category filter
        )
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            // **Modified Query Initialization - Explicitly start as IQueryable<Expense>**
            IQueryable<Expense> query = _context.Expenses
                .Where(e => e.UserId == userId.Value)
                .Include(e => e.Category)
                .AsQueryable(); // Add AsQueryable() to explicitly cast to IQueryable<Expense> first, then Include()

            // Apply date range filter if provided - refine the query
            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate > endDate)
                    return BadRequest("Start date cannot be after end date.");
                query = query.Where(e => e.Date >= startDate.Value.Date && e.Date <= endDate.Value.Date);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value.Date);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value.Date);
            }

            // Apply category filter if provided - refine the query
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            var expenses = await query
                .Select(e => new ExpenseDto // **Corrected - using object initializer again**
                {
                    ExpenseId = e.ExpenseId,
                    Date = e.Date,
                    CategoryName = e.Category.Name,
                    Amount = e.Amount,
                    Description = e.Description
                })
                .ToListAsync();

            return Ok(expenses);
        }

        // GET: api/Expenses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetExpenseById(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var expense = await _context.Expenses
                .Where(e => e.ExpenseId == id && e.UserId == userId.Value)
                .Include(e => e.Category)
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    Date = e.Date,
                    CategoryName = e.Category.Name,
                    Amount = e.Amount,
                    Description = e.Description
                })
                .FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            return Ok(expense);
        }

        // POST: api/Expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense([FromBody] CreateExpenseRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var expense = new Expense()
            {
                UserId = userId.Value,
                CategoryId = model.CategoryId,
                Date = model.Date,
                Amount = model.Amount,
                Description = model.Description
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.ExpenseId }, expense);
        }

        // PUT: api/Expenses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != model.ExpenseId)
            {
                return BadRequest("ExpenseId in request body does not match route parameter.");
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var existingExpense = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId.Value);
            if (existingExpense == null)
            {
                return NotFound();
            }

            existingExpense.CategoryId = model.CategoryId;
            existingExpense.Date = model.Date;
            existingExpense.Amount = model.Amount;
            existingExpense.Description = model.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Expenses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId.Value);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
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