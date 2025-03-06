using BudgetTrackerWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http; // Add using for HttpClient
using System.Text.Json; // Add using for JsonSerializer
using System.Text;
using System.Text.Json.Nodes; // Add using for Encoding

namespace BudgetTrackerWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; // Inject IHttpClientFactory

        public AuthController(IHttpClientFactory httpClientFactory) // Constructor injection
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet] // Add HttpGet attribute for clarity (optional for View-returning actions)
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("BudgetTrackerAPI");
                var jsonContent = JsonSerializer.Serialize(model);
                var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/auth/login", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // **Extract JWT token from API response**
                    JsonNode responseObject = JsonNode.Parse(responseContent); // Parse JSON response
                    string token = responseObject["token"].ToString(); // token key is "token

                    // **Pass token to the View using ViewBag**
                    ViewBag.AuthToken = token;

                    return View("Login"); 
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Login failed: {response.ReasonPhrase}");
                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error during login: {ex.Message}");
                return View(model);
            }
        }


        [HttpGet] // Add HttpGet attribute for clarity
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost] // Indicate that this action handles form POST requests
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("BudgetTrackerAPI");
                var jsonContent = JsonSerializer.Serialize(model); // Serialize RegisterViewModel to JSON
                var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/auth/register", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // Registration successful - redirect to Login page
                    return RedirectToAction("Login");
                }
                else
                {
                    // Handle registration failure - extract error messages
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    // **Basic error handling - improve to parse structured API error response**
                    ModelState.AddModelError(string.Empty, $"Registration failed: {response.ReasonPhrase}"); // Generic error message for now
                    return View(model); // Return Register view with errors
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions during API call
                ModelState.AddModelError(string.Empty, $"Error during registration: {ex.Message}");
                return View(model);
            }
        }
    }
}