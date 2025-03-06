using System.ComponentModel.DataAnnotations;
using BudgetTrackerAPI.Models.Validation;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class BudgetStatusReportRequestDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [AssertThat("EndDateIsAfterStartDate", ErrorMessage = "End Date must be after Start Date.")]
        public bool EndDateIsAfterStartDate => EndDate >= StartDate;
    }
}