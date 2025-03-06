using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User"; // Default role

        public ICollection<Expense> Expenses { get; set; } // Navigation property for Expenses
        public ICollection<Budget> Budgets { get; set; }   // Navigation property for Budgets
        public ICollection<Category> Categories { get; set; } // Navigation property for User-defined Categories
    }
}