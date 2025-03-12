using BudgetTrackerWebApp.Models.ViewModels; // Ensure namespace is correct
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json; // Add this for ReadFromJsonAsync

namespace BudgetTrackerWebApp.Controllers
{
    
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index() // Changed action name to Index for standard convention, route will be /Dashboard/Index or just /Dashboard
        {
            var httpClient = _httpClientFactory.CreateClient("BudgetTrackerApiClient"); // Use your named HttpClient

            try
            {
                var response = await httpClient.GetAsync("api/Dashboard"); // API endpoint for dashboard data

                if (response.IsSuccessStatusCode)
                {
                    var dashboardData = await response.Content.ReadFromJsonAsync<DashboardViewModel>(); // Deserialize JSON to DashboardViewModel
                    if (dashboardData != null)
                    {
                        return View(dashboardData); // Pass the ViewModel to the View
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to deserialize dashboard data from API.";
                    }
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Optionally handle Unauthorized specifically, e.g., redirect to Login page
                        return RedirectToAction("Login", "Auth"); // Or show a specific "Unauthorized" view
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Error fetching dashboard data: Status code {response.StatusCode}";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"HTTP Request Error: {ex.Message}";
                // Log exception for detailed error tracking in real app
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Unexpected error: {ex.Message}";
                // Log exception
            }

            return View(); // Return default View (likely with error message in ViewBag) if data fetch fails
        }
    }
}