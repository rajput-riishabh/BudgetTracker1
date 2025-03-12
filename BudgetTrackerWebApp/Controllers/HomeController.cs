using System.Diagnostics;
using BudgetTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            return RedirectToAction("Index", "Dashboard"); // Redirect to DashboardController's Index action
        }
        public IActionResult Expenses()
        {
            // Redirect to the Index action of the ExpensesController
            return RedirectToAction("Index", "Expenses");
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
        public IActionResult Income()
        {
            return View();
        }
        public IActionResult Profile()
        {
            ViewData["Title"] = "Profile"; // Set page title for Profile view
            return View(); // Will look for Views/Home/Profile.cshtml
        }
    }
}
