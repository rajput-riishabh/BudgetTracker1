namespace BudgetTrackerAPI.Models.DTOs
{
    public class BudgetStatusReportDto
    {
        public string CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal RemainingBudget { get; set; }
        public string Status { get; set; } // "Under Budget", "Approaching Budget", "Budget Exceeded"
    }
}