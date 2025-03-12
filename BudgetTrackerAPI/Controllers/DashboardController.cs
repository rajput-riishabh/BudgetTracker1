using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTrackerAPI.Controllers
{
    [Authorize] // Apply authorization to all actions in this controller
    [ApiController]
    [Route("api/[controller]")] // Route for this controller will be api/Dashboard
    public class DashboardController : ControllerBase
    {
        [HttpGet] // Route: api/Dashboard (GET) - no action name needed now
        public IActionResult GetDashboardData()
        {
            // In a real application, you would fetch actual dashboard data from services/database here
            var dashboardData = new
            {
                WelcomeMessage = "Welcome to your Budget Dashboard, " + User.Identity?.Name, // Example: Get username from token (if available)
                TotalExpensesThisMonth = 1500,
                BudgetRemaining = 500
                // ... more dashboard data ...
            };

            return Ok(dashboardData); // Return sample dashboard data as JSON
        }
    }
}