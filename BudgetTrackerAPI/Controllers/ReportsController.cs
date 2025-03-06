using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // For List<>
using System.Security.Claims;

namespace BudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Secure controller
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports/ExpenseSummaryByCategory
        [HttpGet("ExpenseSummaryByCategory")]
        public async Task<ActionResult<IEnumerable<ExpenseSummaryByCategoryDto>>> GetExpenseSummaryByCategory(
            [FromQuery] ExpenseSummaryByCategoryRequestDto model) // Get date range from query parameters
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Validation errors
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var expenseSummary = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= model.StartDate && e.Date <= model.EndDate) // Filter by user and date range
                .Include(e => e.Category) // Eager load category
                .GroupBy(e => e.Category.Name) // Group by category name
                .Select(group => new ExpenseSummaryByCategoryDto // Project to DTO
                {
                    CategoryName = group.Key, // Category Name (grouping key)
                    TotalExpenses = group.Sum(e => e.Amount) // Sum of amounts in each category
                })
                .ToListAsync();

            return Ok(expenseSummary);
        }

        // GET: api/Reports/BudgetStatusReport
        [HttpGet("BudgetStatusReport")]
        public async Task<ActionResult<IEnumerable<BudgetStatusReportDto>>> GetBudgetStatusReport(
            [FromQuery] BudgetStatusReportRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var budgetStatusReport = new List<BudgetStatusReportDto>();

            // 1. Get all budgets for the user within the specified date range
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId.Value &&
                            b.StartDate <= model.EndDate && b.EndDate >= model.StartDate) // Budgets overlapping the date range
                .Include(b => b.Category)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                // 2. Calculate total expenses for each budget's category within the budget's date range
                var totalExpenses = await _context.Expenses
                    .Where(e => e.UserId == userId.Value &&
                                e.CategoryId == budget.CategoryId &&
                                e.Date >= budget.StartDate && e.Date <= budget.EndDate)
                    .SumAsync(e => e.Amount);

                decimal remainingBudget = budget.Amount - totalExpenses;
                string status = "Under Budget"; // Default status

                if (totalExpenses > budget.Amount)
                {
                    status = "Budget Exceeded";
                }
                else if (remainingBudget < (budget.Amount * 0.20m)) // Approaching budget if remaining is less than 20% (example threshold)
                {
                    status = "Approaching Budget";
                }

                budgetStatusReport.Add(new BudgetStatusReportDto
                {
                    CategoryName = budget.Category.Name,
                    BudgetAmount = budget.Amount,
                    TotalExpenses = totalExpenses,
                    RemainingBudget = remainingBudget,
                    Status = status
                });
            }

            return Ok(budgetStatusReport);
        }

        // GET: api/Reports/IncomeVsExpenseReport
        [HttpGet("IncomeVsExpenseReport")]
        public async Task<ActionResult<IncomeVsExpenseReportDto>> GetIncomeVsExpenseReport(
            [FromQuery] IncomeVsExpenseReportRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (user == null) return NotFound("User not found."); // Should not happen

            // 1. Calculate Total Expenses for the date range (unchanged)
            decimal totalExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= model.StartDate && e.Date <= model.EndDate)
                .SumAsync(e => e.Amount);

            // 2. Calculate Total Income from Income table for the date range
            decimal totalIncome = await _context.Incomes
                .Where(i => i.UserId == userId.Value && i.Date >= model.StartDate && i.Date <= model.EndDate)
                .SumAsync(i => i.Amount);


            decimal netIncomeLoss = totalIncome - totalExpenses;

            var reportDto = new IncomeVsExpenseReportDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetIncomeLoss = netIncomeLoss,
                ReportPeriod = $"{model.StartDate.ToShortDateString()} - {model.EndDate.ToShortDateString()}"
            };

            return Ok(reportDto);
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