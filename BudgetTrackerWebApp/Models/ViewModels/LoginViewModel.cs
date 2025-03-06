using System.ComponentModel.DataAnnotations;

namespace BudgetTrackerWebApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")] // Updated Error Message
        public string UserName { get; set; } // Renamed property to UserName

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}