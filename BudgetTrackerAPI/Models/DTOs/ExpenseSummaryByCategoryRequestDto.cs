using System.ComponentModel.DataAnnotations;
using BudgetTrackerAPI.Models.Validation;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class ExpenseSummaryByCategoryRequestDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [AssertThat("EndDateIsAfterStartDate", ErrorMessage = "End Date must be after Start Date.")] // Custom validation
        public bool EndDateIsAfterStartDate => EndDate >= StartDate; // Custom validation logic
    }
}