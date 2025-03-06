using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerAPI.Models
{
    //[Index(nameof(Name), nameof(UserId), IsUnique = true, Filter = "[UserId] IS NOT NULL")] // Unique index
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int? UserId { get; set; } // Nullable FK, User-defined categories

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public bool IsPredefined { get; set; } = false; // Default to false

        public ICollection<Expense> Expenses { get; set; } // Navigation for Expenses
        public ICollection<Budget> Budgets { get; set; }   // Navigation for Budgets
    }
}