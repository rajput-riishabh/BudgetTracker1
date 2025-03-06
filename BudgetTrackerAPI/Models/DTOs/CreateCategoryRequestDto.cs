using System.ComponentModel.DataAnnotations;

namespace BudgetTrackerAPI.Models.DTOs
{
    public class CreateCategoryRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}