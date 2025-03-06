using System.Diagnostics;
using BudgetTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTrackerWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // **New Placeholder Actions for Sidebar Links:**
        public IActionResult Dashboard()
        {
            return View(); // Returns the Dashboard.cshtml view
        }
        public IActionResult Expenses()
        {
            //ViewData["Title"] = "Expenses"; // Set page title for Expenses view
            return View(); // Will look for Views/Home/Expenses.cshtml
        }

        public IActionResult Budgets()
        {
            ViewData["Title"] = "Budgets"; // Set page title for Budgets view
            return View(); // Will look for Views/Home/Budgets.cshtml
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Reports"; // Set page title for Reports view
            return View(); // Will look for Views/Home/Reports.cshtml
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "Profile"; // Set page title for Profile view
            return View(); // Will look for Views/Home/Profile.cshtml
        }
    }
}
