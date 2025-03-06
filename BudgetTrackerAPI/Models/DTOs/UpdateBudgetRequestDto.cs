using System.ComponentModel.DataAnnotations;
using BudgetTrackerAPI.Models.Validation;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class UpdateBudgetRequestDto
    {
        [Required]
        public int BudgetId { get; set; } // BudgetId for update

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [AssertThat("EndDateIsAfterStartDate", ErrorMessage = "End Date must be after Start Date.")] // Custom validation
        public bool EndDateIsAfterStartDate => EndDate >= StartDate; // Custom validation logic (server-side)
    }
}