namespace BudgetTrackerAPI.Models.DTOs
{
    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public DateTime Date { get; set; }
        public string CategoryName { get; set; } // Category Name instead of CategoryId for response
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}