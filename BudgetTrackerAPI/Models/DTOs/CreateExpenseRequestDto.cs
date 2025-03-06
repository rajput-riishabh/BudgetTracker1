using System.ComponentModel.DataAnnotations;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class CreateExpenseRequestDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}