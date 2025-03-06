using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerAPI.Models
{
    public class Income
    {
        [Key]
        public int IncomeId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign Key to User table
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string Source { get; set; } // Source of income (e.g., Salary, Freelance, Investment)

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } // Optional description
    }
}