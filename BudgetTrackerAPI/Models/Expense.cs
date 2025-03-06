using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerAPI.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // For currency precision
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)] // Specify Date type
        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; } // Nullable description
    }
}