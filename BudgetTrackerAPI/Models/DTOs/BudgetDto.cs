namespace BudgetTrackerAPI.Models.DTOs
{
    public class BudgetDto
    {
        public int BudgetId { get; set; }
        public string CategoryName { get; set; } // Category Name for response
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}