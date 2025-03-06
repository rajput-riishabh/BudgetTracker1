using System.ComponentModel.DataAnnotations;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}