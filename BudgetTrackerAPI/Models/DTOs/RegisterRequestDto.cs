using System.ComponentModel.DataAnnotations;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}