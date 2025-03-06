using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Models;
using BudgetTrackerAPI.Models.DTOs;
using BudgetTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // For PasswordHasher
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Add this for FirstOrDefaultAsync

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

            return Ok(new { Token = token, Message = "Login successful" }); // Return token and success message
        }

        [HttpGet("dashboarddata")] // New endpoint for dashboard data
        [Authorize] //  <--  Protect this endpoint with JWT Authentication
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