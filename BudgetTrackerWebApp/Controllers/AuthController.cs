using BudgetTrackerWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace BudgetTrackerWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
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
                var httpClient = _httpClientFactory.CreateClient("BudgetTrackerApiClient");
                var jsonContent = JsonSerializer.Serialize(model);
                var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/auth/login", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API Login Response Content (Success): {responseContent}"); // Log success response

                    string username = model.UserName; // Use username from login model for claims

                    // Create claims (identity) for the authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username), // Use username from login model!
                        
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API Login Failed. Status Code: {response.StatusCode}, Response: {errorResponse}");
                    ModelState.AddModelError(string.Empty, $"Login failed: {response.ReasonPhrase}. {errorResponse}");
                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HttpRequestException during Login: {ex.Message}, StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"Error during login: {ex.Message}");
                return View(model);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JsonException during Login (parsing API response): {ex.Message}, Response Content might be invalid JSON. StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "Error parsing API login response. It might be invalid.");
                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during Login: {ex.Message}, StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred during login.");
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("BudgetTrackerApiClient");
                var jsonContent = JsonSerializer.Serialize(model);
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
                    ModelState.AddModelError(string.Empty, $"Registration failed: {response.ReasonPhrase}");
                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions during API call
                ModelState.AddModelError(string.Empty, $"Error during registration: {ex.Message}");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // If using cookie-based auth, sign out
            return RedirectToAction("Login", "Auth"); // Redirect to Login page after logout
        }
    }
}