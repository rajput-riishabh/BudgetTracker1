using System.ComponentModel.DataAnnotations;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Current Password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "New Password must be at least 6 characters long")] // Example min length
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}