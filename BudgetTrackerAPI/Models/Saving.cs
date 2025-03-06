using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerAPI.Models
{
    public class Saving
    {
        [Key]
        public int SavingId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign Key to User table
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; } // Name of saving account (e.g., Emergency Fund, Vacation Savings)

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } // Optional description
    }
}