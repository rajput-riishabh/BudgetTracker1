using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Models;
using BudgetTrackerAPI.Models.DTOs;
using BudgetTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // For PasswordHasher
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Add this for FirstOrDefaultAsync
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace BudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService; // Inject JwtService

        // Corrected Constructor - No constructor chaining needed, initialize both directly
        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Check if username or email already exists
            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Username already taken.");
                return BadRequest(ModelState);
            }
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return BadRequest(ModelState);
            }

            // Password hashing
            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(null, model.Password); // No user instance needed for hashing

            // Create new user
            var user = new User()
            {
                Name = model.Name,
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "User" // Default role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful" }); // Return success message
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." }); // User not found
            }

            // Verify password
            var passwordHasher = new PasswordHasher<User>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, user.PasswordHash, model.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid username or password." }); // Invalid password
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            Debug.WriteLine("AuthController.Login: About to set authToken cookie..."); // **DEBUG LOGGING BEFORE**

            // **SET AUTHENTICATION COOKIE HERE**
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Important for security: Cookie cannot be accessed by client-side JavaScript
                Secure = false,   // Recommended: Set to true in production for HTTPS
                SameSite = SameSiteMode.Strict, // Recommended: Helps prevent CSRF attacks
                Expires = DateTime.UtcNow.AddMinutes(60) // Cookie expiration should match token expiration (or be longer if needed)
            };
            HttpContext.Response.Cookies.Append("authToken", token, cookieOptions);

            Debug.WriteLine("AuthController.Login: authToken cookie SET."); // **DEBUG LOGGING AFTER**

            return Ok(new { Message = "Login successful" }); // Remove Token from the JSON response, as it's in the cookie now
        }


        //[AllowAnonymous]
        //[HttpGet("testcookie")]
        //public IActionResult TestCookieSet()
        //{
        //    Debug.WriteLine("AuthController.TestCookieSet: About to set isolated test cookie...");

        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = false,
        //        SameSite = SameSiteMode.None,
        //        Expires = DateTime.UtcNow.AddMinutes(5)
        //    };
        //    HttpContext.Response.Cookies.Append("isolatedTestCookie", "isolatedTestValue", cookieOptions);

        //    Debug.WriteLine("AuthController.TestCookieSet: isolatedTestCookie SET.");
        //    return Ok(new { Message = "Test Cookie Set (Isolated)" });
        //}




        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        //{
        //    Debug.WriteLine("AuthController.Login: Barebones - About to set test cookie...");

        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = false,
        //        SameSite = SameSiteMode.None,
        //        Expires = DateTime.UtcNow.AddMinutes(5)
        //    };
        //    HttpContext.Response.Cookies.Append("testCookie", "testValue", cookieOptions);

        //    Debug.WriteLine("AuthController.Login: Barebones - testCookie SET.");

        //    return Ok(new { Message = "Login successful (Test Cookie Set)" });
        //}
    }
}