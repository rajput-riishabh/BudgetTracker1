using System.Diagnostics;
using BudgetTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using System.Text.Json; // Make sure to install Newtonsoft.Json NuGet package

namespace BudgetTrackerWebApp.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ILogger<ExpensesController> _logger;
        private readonly IHttpClientFactory _clientFactory; // Inject HttpClientFactory

        public ExpensesController(ILogger<ExpensesController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory; // Initialize HttpClientFactory
        }
        // GET: api/Expenses/AddExpense
        [HttpGet("AddExpense")] // Route for displaying the Add Expense view
        public IActionResult AddExpense()
        {
            // For now, simply return the view.
            // In a more complex scenario, you might pass data to the view here.
            return View("AddExpense"); // Returns the AddExpense.cshtml view
        }

        // GET: /Expenses or /Expenses/Index
        public async Task<IActionResult> Index()
        {
            List<Expense> expenses = new List<Expense>();
            string apiUrl = "http://localhost:5285/api/Expenses"; // Your API endpoint

            try
            {
                var client = _clientFactory.CreateClient();
                string authToken = HttpContext.Request.Cookies["authToken"]; // Get token from cookie
                 // **--- ADD THESE LOG LINES ---**
                _logger.LogInformation($"Retrieved authToken from cookie: {authToken}");
                if (string.IsNullOrEmpty(authToken))
                {
                    _logger.LogWarning("AuthToken from cookie is null or empty!");
                }
                // **--- END LOG LINES ---**
                if (!string.IsNullOrEmpty(authToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    expenses = JsonConvert.DeserializeObject<List<Expense>>(jsonString);
                }
                else
                {
                    _logger.LogError($"Failed to fetch expenses from API. Status code: {response.StatusCode}");
                    // Optionally handle error, maybe set an error message in ViewData to display in the view
                    ViewData["ErrorMessage"] = "Failed to load expenses. Please try again later.";
                    return View(expenses); // Return empty list or handle error in view
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request Exception while fetching expenses: {ex.Message}");
                ViewData["ErrorMessage"] = "Error loading expenses. Network error.";
                return View(expenses); // Return empty list or handle error in view
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON Deserialization Exception: {ex.Message}");
                ViewData["ErrorMessage"] = "Error processing expense data.";
                return View(expenses); // Return empty list or handle error in view
            }

            return View(expenses); // Pass the fetched expenses to the View
        }
    }

    // **Create a simple Expense model here (if you don't have one already in Models folder)**
    public class Expense
    {
        public int ExpenseId { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Add CategoryName property
        public string Description { get; set; }
        public decimal Amount { get; set; }
        // Add other properties if needed, matching your API response
    }
}