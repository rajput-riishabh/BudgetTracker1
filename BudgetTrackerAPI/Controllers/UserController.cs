using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Models;
using BudgetTrackerAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // For PasswordHasher
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager; // Inject UserManager

        public UserController(ApplicationDbContext context, UserManager<User> userManager) // Inject UserManager in constructor
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: api/User/profile
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
            {
                return NotFound(); // Should not happen if JWT is valid and user exists
            }

            var profileDto = new UserProfileDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(profileDto);
        }

        // PUT: api/User/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (existingUser == null)
            {
                return NotFound(); // Should not happen if JWT is valid and user exists
            }

            existingUser.Name = model.Name;
            existingUser.Email = model.Email;

            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - successful update
        }

        // PUT: api/User/changepassword
        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId.Value.ToString()); // Use UserManager to find user
            if (user == null) return NotFound("User not found."); // Should not happen

            // 1. Verify current password
            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheckResult)
            {
                return BadRequest(new { message = "Invalid current password." }); // Or Unauthorized (401) depending on desired behavior
            }

            // 2. Hash and update new password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword); // UserManager handles hashing

            if (!changePasswordResult.Succeeded)
            {
                // Password change failed - inspect errors (e.g., password complexity issues - if you configured password policies in Startup.cs)
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState); // Return detailed errors
            }


            return NoContent(); // 204 No Content - successful password change
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