namespace BudgetTrackerAPI.Models.DTOs
{
    public class ExpenseSummaryByCategoryDto
    {
        public string CategoryName { get; set; }
        public decimal TotalExpenses { get; set; }
    }
}