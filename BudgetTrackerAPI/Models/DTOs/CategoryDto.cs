namespace BudgetTrackerAPI.Models.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsPredefined { get; set; }
    }
}