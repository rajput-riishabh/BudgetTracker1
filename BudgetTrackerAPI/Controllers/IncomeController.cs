using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetTrackerAPI.Data; // Make sure to include your Data namespace
using BudgetTrackerAPI.Models; // Make sure to include your Models namespace
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTrackerAPI.Controllers
{
    [Authorize] // Protect this controller with JWT Authentication
    [ApiController]
    [Route("api/[controller]")] // Route for this controller will be api/Income
    public class IncomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null; // Or throw an exception if user ID is always expected
        }

        // GET: api/Income
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Income>>> GetIncomes()
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            // Fetch incomes for the logged-in user
            return await _context.Incomes
                   .Where(i => i.UserId == userId.Value)
                   .ToListAsync();
        }

        // GET: api/Income/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Income>> GetIncome(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);

            if (income == null)
            {
                return NotFound();
            }

            return income;
        }

        // POST: api/Income
        [HttpPost]
        public async Task<ActionResult<Income>> CreateIncome(Income income)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            income.UserId = userId.Value; // Assign user ID from token
            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIncome), new { id = income.IncomeId }, income);
        }

        // PUT: api/Income/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncome(int id, Income income)
        {
            if (id != income.IncomeId)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            if (!IncomeExists(id, userId.Value))
            {
                return NotFound();
            }

            income.UserId = userId.Value; // Ensure UserId is not changed or is correct
            _context.Entry(income).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncomeExists(id, userId.Value))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Income/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var income = await _context.Incomes.FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);
            if (income == null)
            {
                return NotFound();
            }

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IncomeExists(int id, int userId)
        {
            return _context.Incomes.Any(e => e.IncomeId == id && e.UserId == userId);
        }
    }
}