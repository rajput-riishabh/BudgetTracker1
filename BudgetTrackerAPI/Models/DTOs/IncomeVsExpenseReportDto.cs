namespace BudgetTrackerAPI.Models.DTOs
{
    public class IncomeVsExpenseReportDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncomeLoss { get; set; }
        public string ReportPeriod { get; set; } // e.g., "January 1, 2024 - January 31, 2024"
    }
}